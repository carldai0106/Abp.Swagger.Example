using System.Reflection;
using Abp.Modules;

namespace Abp.Swagger
{
    public class SwaggerApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
