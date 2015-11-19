using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Modules;
using Abp.Swagger;
using Abp.Swagger.WebApi;

namespace MvcApp
{
    [DependsOn(
        typeof(SwaggerApplicationModule),
        typeof(SwaggerWebApiModule))]
    public class MvcWebAppModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            SwaggerBuilder.ForAll(typeof(SwaggerApplicationModule).Assembly, "app");

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
