﻿using System;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Web;

namespace MvcApp
{
    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            /* This line provides better startup performance for the application by disabling detailed assembly investigation.
             * If you need deeper assembly investigation, remove it. */
            IocManager.Instance.RegisterIfNot<IAssemblyFinder, CurrentDomainAssemblyFinder>();

            //IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
            base.Application_Start(sender, e);
        }
    }
}
