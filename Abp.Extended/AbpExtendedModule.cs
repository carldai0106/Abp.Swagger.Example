using System.Reflection;
using Abp.Modules;
using Abp.Runtime.Caching;

namespace Abp
{
    public class AbpExtendedModule : AbpModule
    {
        public override void Initialize()
        {
            //IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}