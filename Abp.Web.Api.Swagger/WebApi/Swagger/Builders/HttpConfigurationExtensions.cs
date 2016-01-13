using System;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Routing;
using Abp.WebApi.Swagger.Application;
using Newtonsoft.Json;

namespace Abp.WebApi.Swagger.Builders
{
    public static class HttpConfigurationExtensions
    {
        public static SwaggerEnabledConfiguration EnableSwagger(
            this HttpConfiguration httpConfig
            )
        {
            var config = new SwaggerDocsConfig();
            return new SwaggerEnabledConfiguration(
                httpConfig,
                config.GetRootUrl);
        }

        internal static JsonSerializerSettings SerializerSettingsOrDefault(this HttpConfiguration httpConfig)
        {
            var formatter = httpConfig.Formatters.JsonFormatter;
            if (formatter != null)
                return formatter.SerializerSettings;

            return new JsonSerializerSettings();
        }
    }

    public class SwaggerEnabledConfiguration
    {
        private static readonly string DefaultRouteTemplate = "abpswagger/ui/{*assetPath}";
        private readonly HttpConfiguration _httpConfig;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;
        public SwaggerEnabledConfiguration(
            HttpConfiguration httpConfig,
            Func<HttpRequestMessage, string> rootUrlResolver
            )
        {
            _httpConfig = httpConfig;
            _rootUrlResolver = rootUrlResolver;
        }
       

        public void EnableSwaggerUi(string routeTemplate = "abpswagger/ui/{*assetPath}")
        {
            var config = new SwaggerUiConfig(null, _rootUrlResolver);
            config.DocExpansion(DocExpansion.List);

            _httpConfig.Routes.MapHttpRoute(
                name: "swagger_ui",
                routeTemplate: routeTemplate,
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: new SwaggerUiHandler(config)
            );

            if (routeTemplate == DefaultRouteTemplate)
            {
                _httpConfig.Routes.MapHttpRoute(
                    name: "swagger_ui_shortcut",
                    routeTemplate: "abpswagger",
                    defaults: null,
                    constraints: new { uriResolution = new HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                    handler: new RedirectHandler(_rootUrlResolver, "abpswagger/ui/index"));
            }
        }
    }
}
