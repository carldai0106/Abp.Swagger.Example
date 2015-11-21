/*
 *  感谢yuzukwok  https://github.com/yuzukwok/aspnetboilerplate/tree/master/src/Abp.Web.Api.Swagger
 *  我的ABP Swagger 代码大部分来自该作者
 *  我只是给予了改进
 */
using System.Linq;
using System.Reflection;
using Abp.Application.Services;
using Abp.Reflection.Extensions;
using NJsonSchema;
using NSwag;

namespace Abp.Swagger
{
    public class AssemblyTypeToSwaggerGenerator
    {
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator"/> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public AssemblyTypeToSwaggerGenerator(string assemblyPath)
        {
            _assemblyPath = assemblyPath;
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetAbpServiceBaseClasses()
        {
            var assembly = Assembly.LoadFrom(_assemblyPath);

            return assembly.ExportedTypes.Where(x => x.IsInheritsOrImplements(typeof (ApplicationService)))
                   .Select(x => x.FullName)
                   .ToArray();
        }

        public SwaggerServiceExtended FromAbpApplicationMoudleAssembly(string controllerClassName, string urlTemplate)
        {
            var assembly = Assembly.LoadFrom(_assemblyPath);
            var type = assembly.GetType(controllerClassName);
            var interfaceType = type.GetInterface("I" + type.Name);
            if (interfaceType != null)
            {
                //var map = type.GetInterfaceMap(interfacetype);
                var generator = new AbpServiceBaseToSwaggerGenerator(urlTemplate, new JsonSchemaGeneratorSettings());
                return generator.GenerateForAbpAppService(type, interfaceType);
            }
            
            return null;
        }
       
    }
}
