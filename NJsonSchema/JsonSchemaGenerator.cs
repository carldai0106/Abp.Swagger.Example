﻿//-----------------------------------------------------------------------
// <copyright file="JsonSchemaGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.Infrastructure;

namespace NJsonSchema
{
    /// <summary>Generates a <see cref="JsonSchema4"/> object for a given type. </summary>
    public class JsonSchemaGenerator
    {
        /// <summary>Initializes a new instance of the <see cref="JsonSchemaGenerator"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public JsonSchemaGenerator(JsonSchemaGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets the settings.</summary>
        public JsonSchemaGeneratorSettings Settings { get; private set; }

        /// <summary>Generates a <see cref="JsonSchema4" /> object for the given type and adds the mapping to the given resolver.</summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <returns>The schema.</returns>
        /// <exception cref="InvalidOperationException">Could not find value type of dictionary type.</exception>
        /// <exception cref="InvalidOperationException">Could not find item type of array type.</exception>
        public JsonSchema4 Generate(Type type, ISchemaResolver schemaResolver)
        {
            return Generate<JsonSchema4>(type, schemaResolver);
        }

        /// <summary>Generates a <see cref="JsonSchema4" /> object for the given type and adds the mapping to the given resolver.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <returns>The schema.</returns>
        /// <exception cref="InvalidOperationException">Could not find value type of dictionary type.</exception>
        /// <exception cref="InvalidOperationException">Could not find item type of array type.</exception>
        public TSchemaType Generate<TSchemaType>(Type type, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            return Generate<TSchemaType>(type, null, schemaResolver);
        }

        /// <exception cref="InvalidOperationException">Could not find item type of array type.</exception>
        private TSchemaType Generate<TSchemaType>(Type type, PropertyInfo propertyInfo, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            if (type == typeof(object) || type == typeof(JObject))
            {
                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    AllowAdditionalProperties = true
                };
            }

            var schema = new TSchemaType();

            var typeDescription = JsonObjectTypeDescription.FromType(type);
            schema.Type = typeDescription.Type;
            schema.Format = typeDescription.Format;

            if (schema.Type.HasFlag(JsonObjectType.Object))
            {
                if (typeDescription.IsDictionary)
                    GenerateDictionary(type, schema, schemaResolver);
                else
                {
                    schema.TypeName = GetTypeName(type);

                    if (schemaResolver.HasSchema(type, false))
                    {
                        schema.SchemaReference = schemaResolver.GetSchema(type, false);
                        return schema;
                    }

                    schema.Description = GetDescription(type.GetTypeInfo(), type.GetTypeInfo().GetCustomAttributes());
                    
                    GenerateObject(type, schema, schemaResolver);
                }
            }
            else if (schema.Type.HasFlag(JsonObjectType.Array))
            {
                schema.Type = JsonObjectType.Array;

                var genericTypeArguments = GetGenericTypeArguments(type);
                var itemType = genericTypeArguments.Length == 0 ? type.GetElementType() : genericTypeArguments[0];
                if (itemType == null)
                    throw new InvalidOperationException("Could not find item type of array type '" + type.FullName + "'.");

                schema.Item = Generate<JsonSchema4>(itemType, schemaResolver);
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                var isIntegerEnumeration = IsIntegerEnumeration(propertyInfo);

                if (schemaResolver.HasSchema(type, isIntegerEnumeration))
                {
                    schema.Type = isIntegerEnumeration ? JsonObjectType.Integer : JsonObjectType.String;
                    schema.SchemaReference = schemaResolver.GetSchema(type, isIntegerEnumeration);
                    return schema;
                }

                LoadEnumerations(type, schema, isIntegerEnumeration);

                schema.TypeName = GetTypeName(type); 
                schemaResolver.AddSchema(type, isIntegerEnumeration, schema);
            }

            return schema;
        }

        /// <summary>Checks whether the property is an integer enumeration.</summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>true if the property is an integer enumeration.</returns>
        protected bool IsIntegerEnumeration(PropertyInfo propertyInfo)
        {
            var enumType = Settings.DefaultEnumHandling == EnumHandling.String ? JsonObjectType.String : JsonObjectType.Integer;

            dynamic jsonConverterAttribute = propertyInfo != null ? 
                propertyInfo.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "JsonConverterAttribute") : null;

            if (jsonConverterAttribute != null)
            {
                var converterType = (Type) jsonConverterAttribute.ConverterType;
                if (converterType.Name == "StringEnumConverter")
                {
                    enumType = JsonObjectType.String;
                }
            }

            return enumType == JsonObjectType.Integer;
        }

        private string GetTypeName(Type type)
        {
            if (type.IsConstructedGenericType)
                return type.Name.Split('`').First() + GetTypeName(type.GenericTypeArguments[0]);

            return type.Name;
        }

        /// <exception cref="InvalidOperationException">Could not find value type of dictionary type.</exception>
        private void GenerateDictionary<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            var genericTypeArguments = GetGenericTypeArguments(type);
            if (genericTypeArguments.Length != 2)
                throw new InvalidOperationException("Could not find value type of dictionary type '" + type.FullName + "'.");

            var valueType = genericTypeArguments[1];

            schema.AdditionalPropertiesSchema = Generate<JsonProperty>(valueType, schemaResolver);
            schema.AllowAdditionalProperties = true;
        }

        /// <summary>Gets the generic type arguments of a type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The type arguments.</returns>
        public Type[] GetGenericTypeArguments(Type type)
        {
            var genericTypeArguments = type.GenericTypeArguments;
            while (type != null && type != typeof (object) && genericTypeArguments.Length == 0)
            {
                type = type.GetTypeInfo().BaseType;
                if (type != null)
                    genericTypeArguments = type.GenericTypeArguments;
            }
            return genericTypeArguments;
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        protected virtual void GenerateObject<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            schemaResolver.AddSchema(type, false, schema);
            schema.AllowAdditionalProperties = false;

            GeneratePropertiesAndInheritance(type, schema, schemaResolver);
        }

        private void GeneratePropertiesAndInheritance<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            var properties = GetTypeProperties(type);
            foreach (var property in type.GetTypeInfo().DeclaredProperties.Where(p => properties == null || properties.Contains(p.Name)))
                LoadProperty(property, schema, schemaResolver);

            GenerateInheritance(type, schema, schemaResolver);
        }

        private void GenerateInheritance(Type type, JsonSchema4 schema, ISchemaResolver schemaResolver)
        {
            var baseType = type.GetTypeInfo().BaseType;
            if (baseType != null && baseType != typeof(object))
            {
                if (Settings.FlattenInheritanceHierarchy)
                    GeneratePropertiesAndInheritance(baseType, schema, schemaResolver);
                else
                {
                    var baseSchema = Generate<JsonProperty>(baseType, schemaResolver);
                    schema.AllOf.Add(baseSchema);
                }
            }
        }

        /// <summary>Gets the properties of the given type or null to take all properties.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The property names or null for all.</returns>
        protected virtual string[] GetTypeProperties(Type type)
        {
            if (type == typeof(Exception))
                return new[] { "InnerException", "Message", "Source", "StackTrace" };

            return null; 
        }
        
        private void LoadEnumerations<TSchemaType>(Type type, TSchemaType schema, bool isIntegerEnumeration) 
            where TSchemaType : JsonSchema4, new()
        {
            if (isIntegerEnumeration)
            {
                schema.Type = JsonObjectType.Integer;
                schema.Enumeration.Clear();
                schema.EnumerationNames.Clear();

                foreach (var enumName in Enum.GetNames(type))
                {
                    var value = (int) Enum.Parse(type, enumName);
                    schema.Enumeration.Add(value);
                    schema.EnumerationNames.Add(enumName);
                }
            }
            else
            {
                schema.Type = JsonObjectType.String;
                schema.Enumeration.Clear();
                schema.EnumerationNames.Clear();

                foreach (var enumName in Enum.GetNames(type))
                {
                    schema.Enumeration.Add(enumName);
                    schema.EnumerationNames.Add(enumName);
                }
            }
        }

        private void LoadProperty<TSchemaType>(PropertyInfo property, TSchemaType parentSchema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            var propertyType = property.PropertyType;
            var propertyTypeDescription = JsonObjectTypeDescription.FromType(propertyType);

            var attributes = property.GetCustomAttributes().ToArray();
            if (attributes.All(a => !(a is JsonIgnoreAttribute)))
            {
                var jsonProperty = Generate<JsonProperty>(propertyType, property, schemaResolver);
                var propertyName = JsonPathUtilities.GetPropertyName(property);
                parentSchema.Properties.Add(propertyName, jsonProperty);

                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                var isJsonPropertyRequired = jsonPropertyAttribute != null && jsonPropertyAttribute.Required == Required.Always;

                var requiredAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RequiredAttribute");
                if (propertyTypeDescription.IsAlwaysRequired || requiredAttribute != null || isJsonPropertyRequired)
                    parentSchema.RequiredProperties.Add(propertyName);

                dynamic displayAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.DisplayAttribute");
                if (displayAttribute != null && displayAttribute.Name != null)
                    jsonProperty.Title = displayAttribute.Name;

                jsonProperty.Description = GetDescription(property, attributes);

                dynamic regexAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RegularExpressionAttribute");
                if (regexAttribute != null)
                    jsonProperty.Pattern = regexAttribute.Pattern;

                if (propertyTypeDescription.Type == JsonObjectType.Number || propertyTypeDescription.Type == JsonObjectType.Integer)
                {
                    dynamic rangeAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RangeAttribute");
                    if (rangeAttribute != null)
                    {
                        if (rangeAttribute.Minimum != null)
                            jsonProperty.Minimum = rangeAttribute.Minimum;
                        if (rangeAttribute.Maximum != null)
                            jsonProperty.Maximum = rangeAttribute.Maximum;
                    }
                }

                dynamic minLengthAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.MinLengthAttribute");
                if (minLengthAttribute != null && minLengthAttribute.Length != null)
                {
                    if (propertyTypeDescription.Type == JsonObjectType.String)
                        jsonProperty.MinLength = minLengthAttribute.Length;
                    else if (propertyTypeDescription.Type == JsonObjectType.Array)
                        jsonProperty.MinItems = minLengthAttribute.Length;
                }

                dynamic maxLengthAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.MaxLengthAttribute");
                if (maxLengthAttribute != null && maxLengthAttribute.Length != null)
                {
                    if (propertyTypeDescription.Type == JsonObjectType.String)
                        jsonProperty.MaxLength = maxLengthAttribute.Length;
                    else if (propertyTypeDescription.Type == JsonObjectType.Array)
                        jsonProperty.MaxItems = maxLengthAttribute.Length;
                }
            }
        }

        private string GetDescription(MemberInfo memberInfo, IEnumerable<Attribute> attributes)
        {
            dynamic descriptionAttribute = TryGetAttribute(attributes, "System.ComponentModel.DescriptionAttribute");
            if (descriptionAttribute != null && descriptionAttribute.Description != null)
                return descriptionAttribute.Description;
            else
            {
                dynamic displayAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.DisplayAttribute");
                if (displayAttribute != null && displayAttribute.Description != null)
                    return displayAttribute.Description;
                else
                {
                    var summary = memberInfo.GetXmlDocumentation();
                    if (summary != string.Empty)
                        return summary;
                }
            }

            return null; 
        }

        private Attribute TryGetAttribute(IEnumerable<Attribute> attributes, string attributeType)
        {
            return attributes.FirstOrDefault(a => a.GetType().FullName == attributeType);
        }
    }
}
