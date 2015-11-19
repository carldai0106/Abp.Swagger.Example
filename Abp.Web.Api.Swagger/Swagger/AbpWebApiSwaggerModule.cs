using Abp.Modules;
using Abp.WebApi;

namespace Abp.Swagger
{
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiSwaggerModule : AbpModule
    {
     
    }
}
