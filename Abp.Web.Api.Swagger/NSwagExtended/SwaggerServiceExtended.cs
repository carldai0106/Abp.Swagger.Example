using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.Collections;

namespace Abp.NSwagExtended
{
    public class SwaggerServiceExtended : SwaggerService
    {
        public SwaggerServiceExtended()
        {
            Tags = new List<SwaggerTagInfo>();
            SecurityDefinitions = new Dictionary<string, SwaggerSecuritySchemeExtended>();

            Paths = new ObservableDictionary<string, SwaggerOperationsExtended>();
            Paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };
        }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new List<SwaggerTagInfo> Tags { get; set; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securityDefinitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new Dictionary<string, SwaggerSecuritySchemeExtended> SecurityDefinitions { get; private set; }

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new ObservableDictionary<string, SwaggerOperationsExtended> Paths { get; private set; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new List<SwaggerParameterExtended> Parameters { get; set; }


        /// <summary>Converts the description object to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public string ToJson(JsonConverter converter = null)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };

            if (converter != null)
            {
                settings.Converters.Add(converter);
            }

            GenerateOperationIds();

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            var dic = new Dictionary<string, List<JsonSchema4>>();
            var list = new List<string>();

            var schemes = this.Definitions;

            foreach (var item in schemes)
            {
                var schema = item.Value;
                var name = GetJsonSchema4TypeName(schema, "");
                if (!list.Any(x => x.Split(',').Any(y => name.Split(',').Contains(y))))
                {
                    list.Add(name);
                }
            }

            foreach (var schema in schemes.Select(item => item.Value))
            {
                MakeJsonSchema4Group(schema, list, dic);
            }

            Definitions.Clear();

            foreach (var item in dic)
            {
                var key = item.Key.Split(',').First();
                var info = item.Value.First();
                var lst = item.Value.Where((x, index) => index > 0);
                foreach (var schema in lst)
                {
                    foreach (var it in schema.RequiredProperties)
                    {
                        info.RequiredProperties.Add(it);
                    }

                    foreach (var pair in schema.Properties.Select(it => new KeyValuePair<string, JsonProperty>(it.Key, it.Value)))
                    {
                        info.Properties.Add(pair);
                    }
                }

                Definitions[key] = info;
            }


            var data = JsonConvert.SerializeObject(this, settings);

            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }

        /// <summary>
        /// Get names of all derived types; 
        /// </summary>
        private static string GetJsonSchema4TypeName(JsonSchema4 schema, string sourceName)
        {
            while (true)
            {
                var strName = sourceName;
                if (schema.AllOf.Any())
                {
                    var name = schema.TypeName;
                    if (string.IsNullOrEmpty(strName))
                        strName = name;
                    else
                        strName = strName + "," + name;
                    schema = schema.AllOf.ElementAt(0);
                    sourceName = strName;
                }
                else
                {
                    var name = schema.TypeName;
                    if (string.IsNullOrEmpty(strName))
                        strName = name;
                    else
                        strName = strName + "," + name;
                    return strName;
                }
            }
        }

        /// <summary>
        /// Make JsonSchema4 group by type name.
        /// </summary>
        private static void MakeJsonSchema4Group(JsonSchema4 schema, List<string> list, Dictionary<string, List<JsonSchema4>> dic)
        {
            while (true)
            {
                var name = schema.TypeName;

                var source = JsonConvert.SerializeObject(schema);
                var obj = JsonConvert.DeserializeObject<JsonSchema4>(source);

                if (schema.AllOf.Any())
                {
                    if (!(schema is JsonProperty) && list.Any(x => x.Split(',').Any(y => name.Split(',').Contains(y))))
                    {
                        var key = list.FirstOrDefault(x => x.Split(',').Any(y => name.Split(',').Contains(y))) ?? name;

                        obj.AllOf.Clear();
                        if (obj.Properties.Values.Count > 0)
                        {
                            foreach (var item in obj.Properties.Values.Where(item => item.Item != null))
                            {
                                item.Item.AllOf.Clear();
                            }
                        }

                        List<JsonSchema4> value;
                        if (dic.TryGetValue(key, out value))
                        {
                            value.Add(obj);
                        }
                        else
                        {
                            dic.Add(key, new List<JsonSchema4> {obj});
                        }
                    }

                    schema = schema.AllOf.ElementAt(0);
                    continue;
                }
                if (!(schema is JsonProperty) && list.Any(x => x.Split(',').Any(y => name.Split(',').Contains(y))))
                {
                    var key = list.FirstOrDefault(x => x.Split(',').Any(y => name.Split(',').Contains(y))) ?? name;

                    if (obj.Properties.Values.Count > 0)
                    {
                        foreach (var item in obj.Properties.Values.Where(item => item.Item != null))
                        {
                            item.Item.AllOf.Clear();
                        }
                    }

                    List<JsonSchema4> value;
                    if (dic.TryGetValue(key, out value))
                    {
                        value.Add(obj);
                    }
                    else
                    {
                        dic.Add(key, new List<JsonSchema4> {obj});
                    }
                }
                break;
            }
        }
    }
}
