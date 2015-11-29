using Abp.Modules;
using Abp.WebApi;

namespace Abp.Builders
{
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiSwaggerModule : AbpModule
    {
     
    }
}
