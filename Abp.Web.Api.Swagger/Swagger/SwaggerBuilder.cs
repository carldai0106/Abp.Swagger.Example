/*
 *  感谢yuzukwok  https://github.com/yuzukwok/aspnetboilerplate/tree/master/src/Abp.Web.Api.Swagger
 *  我的ABP Swagger 代码大部分来自该作者
 *  我只是给予了改进
 */
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using NSwag;

namespace Abp.Swagger
{
    public static class SwaggerBuilder
    {
        private const string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{id}";

        public static IDictionary<string, string> ContentJson = new Dictionary<string, string>();

        public static void ForAll(Assembly assembly, string servicePrefix)
        {
            var generator = new AssemblyTypeToSwaggerGenerator(assembly.CodeBase);
            var allClassNames = generator.GetAbpServiceBaseClasses();
            foreach (var item in allClassNames)
            {
                var service = generator.FromAbpApplicationMoudleAssembly(item, UrlTemplate.Replace("{servicePrefix}", servicePrefix));
                if (service == null)
                    continue;

                var converter = new SwaggerOperationsExtendedConverter(typeof(SwaggerOperationsExtended));

                var jsontext = service.ToJson(converter);

                //gen json file 
                if (HttpContext.Current != null)
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/apidoc/"));
                    var file = HttpContext.Current.Server.MapPath("~/apidoc/") + "/" + item + ".js";
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    var writer = File.CreateText(file);
                    writer.Write(jsontext);
                    writer.Close();
                }
            }
        }
    }
}
