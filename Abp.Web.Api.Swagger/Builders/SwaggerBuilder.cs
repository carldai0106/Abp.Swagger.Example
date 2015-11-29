/*
 *  Thaks yuzukwok  https://github.com/yuzukwok/aspnetboilerplate/tree/master/src/Abp.Web.Api.Swagger
 *  According yuzukok's Abp.Web.Api.Swagger project, I make to improve my Abp.Web.Api.Swagger.
 *
 */
using System.Reflection;

namespace Abp.Builders
{
    public static class SwaggerBuilder
    {
        private const string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{id}";

        public static IBatchSwaggerBuilder<T> ForAll<T>(Assembly assembly, string servicePrefix = "app")
        {
            return new BatchSwaggerBuilder<T>(assembly, servicePrefix);
        }
    }
}
