using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Abp.NSwagExtended;
using NJsonSchema;

namespace Abp.Builders
{
    internal class BatchSwaggerBuilder<T> : IBatchSwaggerBuilder<T>
    {
        private const string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{id}";
        private readonly string _servicePrefix;
        private readonly Assembly _assembly;

        public BatchSwaggerBuilder(Assembly assembly, string servicePrefix)
        {
            _assembly = assembly;
            _servicePrefix = servicePrefix;
        }

        public void Build()
        {
            var constNames = new [] { "ApplicationService", "AppService", "Service" };

            var assembly = Assembly.LoadFrom(_assembly.CodeBase);

            var classes = assembly.ExportedTypes.Where(x => typeof(T).IsAssignableFrom(x)).Select(x => x.FullName);

            var urlTemplate = UrlTemplate.Replace("{servicePrefix}", _servicePrefix);

            var rootPath = GetAppPath() + "/WebApiDoc";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            foreach (var item in classes)
            {
                var type = assembly.GetType(item);
                var service = GetSwaggerService(type, urlTemplate);

                if (service == null)
                    continue;

                var converter = new SwaggerOperationsExtendedConverter(typeof(SwaggerOperationsExtended));
                var json = service.ToJson(converter);

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

        private static SwaggerServiceExtended GetSwaggerService(Type type, string urlTemplate)
        {
            var interfaceType = type.GetInterface("I" + type.Name);
            if (interfaceType == null)
            {
                return null;
            }

            var generator = new AbpServiceBaseToSwaggerGenerator(urlTemplate, new JsonSchemaGeneratorSettings());
            return generator.GenerateForAbpAppService(type, interfaceType);
        }

        private static string GetAppPath()
        {
            var path = HttpRuntime.AppDomainAppPath;
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            }

            return path;
        }
    }
}
