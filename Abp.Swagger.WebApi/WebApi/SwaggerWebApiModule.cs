using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace Abp.Swagger.WebApi
{
    [DependsOn(typeof(AbpWebApiModule), typeof(SwaggerApplicationModule))]
    public class SwaggerWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(SwaggerApplicationModule).Assembly, "app")
                .WithConventionalVerbs()
                .Build();
        }
    }
}
