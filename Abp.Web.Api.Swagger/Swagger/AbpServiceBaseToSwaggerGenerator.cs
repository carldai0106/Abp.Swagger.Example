/*
 *  感谢yuzukwok  https://github.com/yuzukwok/aspnetboilerplate/tree/master/src/Abp.Web.Api.Swagger
 *  我的ABP Swagger 代码大部分来自该作者
 *  我只是给予了改进
 */

using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NJsonSchema.Infrastructure;

namespace Abp.Swagger
{
    public class AbpServiceBaseToSwaggerGenerator
    {
        private SwaggerServiceExtended _service;
        private Type _serviceType;
        //private JsonSchema4 _exceptionType;

        /// <summary>Initializes a new instance of the <see cref="AbpServiceBaseToSwaggerGenerator" /> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        /// <param name="jsonSchemaGeneratorSettings">The JSON Schema generator settings.</param>
        public AbpServiceBaseToSwaggerGenerator(string defaultRouteTemplate, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            DefaultRouteTemplate = defaultRouteTemplate;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings;
        }

        /// <summary>Gets or sets the default route template which is used when no route attributes are found (default: 'api/{controller}/{action}/{id}').</summary>
        public string DefaultRouteTemplate { get; set; }

        /// <summary>Gets or sets the JSON Schema generator settings.</summary>
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }

        /// <summary>
        /// 生成文档结构
        /// </summary>
        /// <param name="type">具体的 AppService 类型</param>
        /// <param name="interfaceType">具体的 IAppService 类型</param>
        /// <param name="excludedMethodName">排除的方法名称</param>
        /// <returns></returns>
        public SwaggerServiceExtended GenerateForAbpAppService(Type type, Type interfaceType,
            string excludedMethodName = "Swagger")
        {
            _service = new SwaggerServiceExtended();
            _serviceType = type;
            var schemaResolver = new SchemaResolver();

            var deriveMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var methods = interfaceType.GetMethods();

            dynamic webApiDesc = interfaceType.GetCustomAttributes()
                .FirstOrDefault(x => x.GetType().Name == "WebApiDescriptionAttribute");

            if (webApiDesc != null)
            {
                dynamic name = webApiDesc.Name;
                dynamic description = webApiDesc.Description;

                _service.Tags.Add(new SwaggerTagInfo
                {
                    Name = name,
                    Description = description
                });
            }

            var hasGenerated = false;
            foreach (var method in methods.Where(m => m.Name != excludedMethodName))
            {
                //根据特性判断是否是公共的WebAPI方法
                var canOpen = method.GetCustomAttributes().Any(x => x.GetType().Name == "OpenWebApiAttribute");
                if (!canOpen)
                    continue;

                var driveMethod = GetSpecifiedMethod(deriveMethods, method);
                var parameters = method.GetParameters().ToList();
                var methodName = method.Name;

                var operation = new SwaggerOperationExtended { OperationId = methodName };

                dynamic webApiMethodDesc = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().Name == "WebApiDescriptionAttribute");
                if (webApiMethodDesc != null)
                {
                    operation.Tags.Add(webApiMethodDesc.Name);
                }

                var httpPath = GetHttpPath(operation, driveMethod, parameters, schemaResolver);

                LoadParameters(operation, parameters, schemaResolver);

                LoadReturnType(operation, driveMethod, schemaResolver);

                LoadMetaData(operation, method);

                var httpMethod = GetMethod(method);

                if (!_service.Paths.ContainsKey(httpPath))
                {
                    var path = new SwaggerOperationsExtended();
                    _service.Paths[httpPath] = path;
                }

                _service.Paths[httpPath][httpMethod] = operation;

                hasGenerated = true;
            }

            foreach (var schema in schemaResolver.Schemes)
            {
                _service.Definitions[schema.TypeName] = schema;
            }
          
            _service.GenerateOperationIds();

            if (!hasGenerated)
                _service = null;

            return _service;
        }


        private MethodInfo GetSpecifiedMethod(MethodInfo[] driveMethods, MethodInfo interfaceMethod)
        {
            var matchingMethods = driveMethods.Where(mi =>
               mi.ReturnType == interfaceMethod.ReturnType
               && mi.Name == interfaceMethod.Name
               && mi.GetParameters().Select(pi => pi.ParameterType)
                  .SequenceEqual(interfaceMethod.GetParameters().Select(pi => pi.ParameterType)));

            return matchingMethods.FirstOrDefault();
        }

        private void LoadMetaData(SwaggerOperationExtended operation, MethodInfo method)
        {
            dynamic descriptionAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "WebApiDescriptionAttribute");

            if (descriptionAttribute != null)
                operation.Summary = descriptionAttribute.Description;
            else
            {
                var summary = method.GetXmlDocumentation();
                if (summary != string.Empty)
                    operation.Summary = summary;
            }
        }

        private string GetHttpPath(SwaggerOperationExtended operation, MethodInfo method, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            string httpPath;

            dynamic routeAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttribute != null)
            {
                dynamic routePrefixAttribute = _serviceType.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                if (routePrefixAttribute != null)
                    httpPath = routePrefixAttribute.Prefix + "/" + routeAttribute.Template;
                else
                    httpPath = routeAttribute.Template;
            }
            else
            {
                httpPath = DefaultRouteTemplate
                    .Replace("{controller}", _serviceType.Name.Replace("AppService", string.Empty))
                    .Replace("{action}", method.Name);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameter = parameters.SingleOrDefault(p => p.Name == match.Groups[1].Value);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKindExtended.Path;

                    operation.Parameters.Add(operationParameter);
                    parameters.Remove(parameter);
                }
                else
                {
                    httpPath = "/" + httpPath
                        .Replace(match.Value, string.Empty)
                        .Replace("//", "/")
                        .Trim('/');
                }
            }

            return httpPath;
        }

        private SwaggerOperationMethod GetMethod(MethodInfo method)
        {
            var methodName = method.Name;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPostAttribute"))
                return SwaggerOperationMethod.Post;
            else if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpGetAttribute"))
                return SwaggerOperationMethod.Get;
            else if (methodName == "Get")
                return SwaggerOperationMethod.Get;
            else if (methodName == "Post")
                return SwaggerOperationMethod.Post;
            else if (methodName == "Put")
                return SwaggerOperationMethod.Put;
            else if (methodName == "Delete")
                return SwaggerOperationMethod.Delete;
            else
                return SwaggerOperationMethod.Post;
        }


        private void LoadParameters(SwaggerOperationExtended operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
            {
                var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);

                dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                dynamic fromUriAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromUriAttribute");

                // TODO: Add support for ModelBinder attribute

                var isComplexParameter = IsComplexType(info);
                if (isComplexParameter)
                {
                    if (fromUriAttribute != null)
                        AddPrimitiveParameter(operation, schemaResolver, parameter);
                    else
                        AddBodyParameter(operation, schemaResolver, parameter);
                }
                else
                {
                    if (fromBodyAttribute != null)
                        AddBodyParameter(operation, schemaResolver, parameter);
                    else
                        AddPrimitiveParameter(operation, schemaResolver, parameter);
                }
            }

            if (operation.Parameters.Count(p => p.Kind == SwaggerParameterKindExtended.Body) > 1)
                throw new InvalidOperationException("The operation '" + operation.OperationId + "' has more than one body parameter.");
        }

        private void AddBodyParameter(SwaggerOperationExtended operation, ISchemaResolver schemaResolver,
           ParameterInfo parameter)
        {
            var operationParameter = CreateBodyParameter(parameter, schemaResolver);
            operation.Parameters.Add(operationParameter);
        }

        private void AddPrimitiveParameter(SwaggerOperationExtended operation, ISchemaResolver schemaResolver,
            ParameterInfo parameter)
        {
            var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
            operationParameter.Kind = SwaggerParameterKindExtended.Query;
            operation.Parameters.Add(operationParameter);
        }

        private SwaggerParameterExtended CreateBodyParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var operationParameter = new SwaggerParameterExtended
            {
                Schema = CreateAndAddSchema<SwaggerParameterExtended>(parameter.ParameterType, schemaResolver),
                //modify : modify by carl
                //Name = "request";
                Name = parameter.Name,
                
                Kind = SwaggerParameterKindExtended.Body
            };

            return operationParameter;
        }

        private void LoadReturnType(SwaggerOperationExtended operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            if (method.ReturnType.FullName != "System.Void" && method.ReturnType.FullName != "System.Threading.Tasks.Task")
            {
                operation.Responses["200"] = new SwaggerResponse
                {
                    Schema = CreateAndAddSchema<JsonSchema4>(method.ReturnType, schemaResolver)
                };
            }
            else
                operation.Responses["200"] = new SwaggerResponse();
        }

        private TSchemaType CreateAndAddSchema<TSchemaType>(Type type, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            var info = JsonObjectTypeDescription.FromType(type);
            if (info.Type.HasFlag(JsonObjectType.Object))
            {
                if (type == typeof(object))
                {
                    return new TSchemaType
                    {
                        Type = JsonObjectType.Object,
                        AllowAdditionalProperties = false
                    };
                }

                if (!schemaResolver.HasSchema(type))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    SchemaReference = schemaResolver.GetSchema(type)
                };
            }

            if (info.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                return new TSchemaType
                {
                    Type = JsonObjectType.Array,
                    Item = CreateAndAddSchema<JsonSchema4>(itemType, schemaResolver)
                };
            }

            var generator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        private SwaggerParameterExtended CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var parameterGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);

            var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
            var isComplexParameter = IsComplexType(info);
            var parameterType = isComplexParameter ? typeof(string) : parameter.ParameterType; // complex types must be treated as string

            var segmentParameter = parameterGenerator.Generate<SwaggerParameterExtended>(parameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;

            return segmentParameter;
        }

        private bool IsComplexType(JsonObjectTypeDescription info)
        {
            // TODO: Move to JsonObjectTypeDescription class in NJsonSchema
            return info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array);
        }

        //private TSchemaType CreateAndAddSchema<TSchemaType>(Type type, ISchemaResolver schemaResolver)
        //    where TSchemaType : JsonSchema4, new()
        //{
        //    if (type.Name == "Task`1")
        //        type = type.GenericTypeArguments[0];

        //    if (type.Name == "JsonResult`1")
        //        type = type.GenericTypeArguments[0];

        //    var info = JsonObjectTypeDescription.FromType(type);

        //    if (info.Type.HasFlag(JsonObjectType.Object))
        //    {
        //        if (!schemaResolver.HasSchema(type))
        //        {
        //            var schemaGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
        //            schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
        //        }

        //        //load 
        //        //目前先屏蔽处理某些特定类型的异常
        //        //Dictionary<string,string>
        //        JsonSchema4 t = null;
        //        try
        //        {
        //            t = schemaResolver.GetSchema(type);
        //        }
        //        catch (Exception)
        //        {

        //        }
        //        return new TSchemaType
        //        {
        //            Type = JsonObjectType.Object,
        //            SchemaReference = t
        //        };
        //    }

        //    if (info.Type.HasFlag(JsonObjectType.Array))
        //    {
        //        var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
        //        return new TSchemaType
        //        {
        //            Type = JsonObjectType.Array,
        //            Item = CreateAndAddSchema<JsonSchema4>(itemType, schemaResolver)
        //        };
        //    }

        //    var generator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
        //    return generator.Generate<TSchemaType>(type, schemaResolver);
        //}

        //<exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        //private SwaggerParameter CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        //{
        //    var parameterType = parameter.ParameterType;

        //    //处理Guid
        //    if (parameterType == typeof(Guid))
        //    {
        //        parameterType = typeof(string);
        //    }

        //    var info = JsonObjectTypeDescription.FromType(parameterType);
        //    if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
        //        throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array.");

        //    var parameterGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);

        //    var segmentParameter = parameterGenerator.Generate<SwaggerParameter>(parameter.ParameterType, schemaResolver);
        //    segmentParameter.Name = parameter.Name;
        //    return segmentParameter;
        //}


        //public SwaggerService Generate(Type type, InterfaceMapping map, string excludedMethodName = "Swagger")
        //{
        //    _service = new SwaggerService();
        //    _serviceType = type;

        //    //_exceptionType = new JsonSchema4 {TypeName = "SwaggerException"};
        //    //_exceptionType.Properties.Add("ExceptionType", new JsonProperty { Type = JsonObjectType.String });
        //    //_exceptionType.Properties.Add("Message", new JsonProperty { Type = JsonObjectType.String });
        //    //_exceptionType.Properties.Add("StackTrace", new JsonProperty { Type = JsonObjectType.String });

        //    //_service.Definitions[_exceptionType.TypeName] = _exceptionType;

        //    //type.GetCustomAttributes

        //    var schemaResolver = new SchemaResolver();
        //    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);


        //    var hasGenerated = false;
        //    foreach (var method in methods.Where(m => m.Name != excludedMethodName))
        //    {
        //        if (Array.IndexOf(map.TargetMethods, method) == -1)
        //        {
        //            continue;
        //        }
        //        //取得相同的方法名称，以及参数个数
        //        var list = map.InterfaceMethods.Where(x => x.Name == method.Name && x.GetParameters().Count() == method.GetParameters().Count());

        //        //只有当前方法上面有WebApiAttribute特性才允许添加到文档说明
        //        if (!list.Any(x => x.GetCustomAttributes().Any(m => m.GetType() == typeof(WebApiAttribute))))
        //        {
        //            continue;
        //        }

        //        var parameters = method.GetParameters().ToList();
        //        var methodName = method.Name;

        //        var operation = new SwaggerOperation { OperationId = methodName };

        //        var httpPath = GetHttpPath(operation, method, parameters, schemaResolver);

        //        LoadParameters(operation, parameters, schemaResolver);
        //        LoadReturnType(operation, method, schemaResolver);
        //        LoadMetaData(operation, method);

        //        var httpMethod = SwaggerOperationMethod.Post;
        //        //判断是否有同名方法
        //        if (list.Count() == 1)
        //        {
        //            httpMethod = GetMethod(list.FirstOrDefault());
        //        }
        //        else
        //        {
        //            //MethodInfo info = null;
        //            //foreach (var item in list)
        //            //{
        //            //    var pars = item.GetParameters();
        //            //    var count = pars.Where((current, index) => parameters[index].ParameterType == current.ParameterType).Count();

        //            //    if (count != pars.Length) continue;
        //            //    info = item;
        //            //    break;
        //            //}
        //            //如果有同名方法，判断参数类型，以及参数顺序
        //            var info = (from item in list
        //                        let pars = item.GetParameters()
        //                        let count = pars.Where((current, index) => parameters[index].ParameterType == current.ParameterType).Count()
        //                        where count == pars.Length
        //                        select item
        //                               ).FirstOrDefault();

        //            httpMethod = GetMethod(info);
        //        }

        //        if (!_service.Paths.ContainsKey(httpPath))
        //        {
        //            var path = new SwaggerOperations();
        //            _service.Paths[httpPath] = path;
        //        }

        //        _service.Paths[httpPath][httpMethod] = operation;

        //        hasGenerated = true;
        //    }

        //    foreach (var schema in schemaResolver.Schemes)
        //    {
        //        schema.AllOf.Clear();

        //        _service.Definitions[schema.TypeName] = schema;
        //    }

        //    _service.GenerateOperationIds();

        //    if (!hasGenerated)
        //        _service = null;

        //    return _service;
        //}

        // <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        //private void LoadParameters(SwaggerOperation operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        //{
        //    foreach (var parameter in parameters)
        //    {
        //        dynamic fromBodyAttribute = parameter.GetCustomAttributes()
        //            .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

        //        if (fromBodyAttribute != null)
        //        {
        //            var operationParameter = CreateBodyParameter(parameter, schemaResolver);
        //            operation.Parameters.Add(operationParameter);
        //        }
        //        else
        //        {
        //            var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
        //            if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
        //            {
        //                if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.Body))
        //                    throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array. ");

        //                var operationParameter = CreateBodyParameter(parameter, schemaResolver);
        //                operation.Parameters.Add(operationParameter);
        //            }
        //            else
        //            {
        //                var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
        //                operationParameter.Kind = SwaggerParameterKind.Query;

        //                operation.Parameters.Add(operationParameter);
        //            }
        //        }
        //    }
        //}



    }
}
