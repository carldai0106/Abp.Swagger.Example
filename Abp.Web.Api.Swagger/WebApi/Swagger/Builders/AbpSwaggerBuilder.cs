using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Abp.WebApi.Swagger.Adapter;
using NJsonSchema;

namespace Abp.WebApi.Swagger.Builders
{
    public class AbpSwaggerBuilder
    {
        private const string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{id}";

        public static void ForAll<T>(Assembly assembly, string servicePrefix = "app")
        {
            var constNames = new[] { "ApplicationService", "AppService", "Service" };

            var  loadedAssembly = Assembly.LoadFrom(assembly.CodeBase);

            var classes = assembly.ExportedTypes.Where(x => typeof(T).IsAssignableFrom(x)).Select(x => x.FullName);

            var urlTemplate = UrlTemplate.Replace("{servicePrefix}", servicePrefix);

            var rootPath = GetAppPath() + "/WebApiDoc";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            foreach (var item in classes)
            {
                var type = loadedAssembly.GetType(item);
                var service = GetSwaggerService(type, urlTemplate);

                if (service == null)
                    continue;

                var json = service.ToJson();

                var arrs = item.Split('.').ToList();

                var className = string.Empty;
                var subDir = string.Empty;

                if (arrs.Any())
                {
                    className = arrs.LastOrDefault();
                    subDir = item.Replace("." + className, "").Replace(".", "_");
                    if (!string.IsNullOrEmpty(className))
                    {
                        foreach (var name in constNames.Where(name => className.Contains(name)))
                        {
                            className = className.Replace(name, "");
                        }
                    }
                }

                var docPath = rootPath + "/" + subDir;
                if (!Directory.Exists(docPath))
                {
                    Directory.CreateDirectory(docPath);
                }

                var docFullName = docPath + "/" + className + ".js";
                if (File.Exists(docFullName))
                {
                    File.Delete(docFullName);
                }

                using (var writer = File.CreateText(docFullName))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private static AbpSwaggerService GetSwaggerService(Type type, string urlTemplate)
        {
            var interfaceType = type.GetInterface("I" + type.Name);
            if (interfaceType == null)
            {
                return null;
            }

            var generator = new AbpSwaggerGenerator(urlTemplate, new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true
            });
            return generator.GenerateForAbpAppService(type, interfaceType);
        }

        private static string GetAppPath()
        {
            var path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            return path;
        }
    }
}
