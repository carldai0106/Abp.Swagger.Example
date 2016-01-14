using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            return new SwaggerEnabledConfiguration(
                httpConfig,
                DefaultRootUrlResolver);
        }

        /// <summary>
        /// According client request to get root url
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns></returns>
        public static string DefaultRootUrlResolver(HttpRequestMessage request)
        {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            var host = GetHeaderValue(request, "X-Forwarded-Host") ?? request.RequestUri.Host;
            var port = GetHeaderValue(request, "X-Forwarded-Port") ?? request.RequestUri.Port.ToString(CultureInfo.InvariantCulture);

            var httpConfiguration = request.GetConfiguration();
            var virtualPathRoot = httpConfiguration.VirtualPathRoot.TrimEnd('/');

            return $"{scheme}://{host}:{port}{virtualPathRoot}";
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }
    }

    public class SwaggerEnabledConfiguration
    {
        private static readonly string DefaultRouteTemplate = "abpswg/ui/{*assetPath}";
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
       

        public void EnableSwaggerUi(string routeTemplate = "abpswg/ui/{*assetPath}")
        {
            var config = new SwaggerUiConfig(_rootUrlResolver);

            config.DocExpansion(DocExpansion.List);

            _httpConfig.Routes.MapHttpRoute(
                name: "apbswg_ui",
                routeTemplate: routeTemplate,
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: new SwaggerUiHandler(config)
            );
            
            if (routeTemplate == DefaultRouteTemplate)
            {
                _httpConfig.Routes.MapHttpRoute(
                    name: "abpswg_ui_shortcut",
                    routeTemplate: "abpswg",
                    defaults: null,
                    constraints: new { uriResolution = new HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                    handler: new RedirectHandler(_rootUrlResolver, "abpswg/ui/index"));
            }
        }
    }
}
