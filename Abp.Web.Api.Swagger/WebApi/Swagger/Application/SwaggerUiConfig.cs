using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Abp.WebApi.Swagger.Ui;

namespace Abp.WebApi.Swagger.Application
{
    public class SwaggerUiConfig
    {
        private readonly Dictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly Dictionary<string, string> _templateParams;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        public SwaggerUiConfig(IEnumerable<string> discoveryPaths, Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _pathToAssetMap = new Dictionary<string, EmbeddedAssetDescriptor>();

            _templateParams = new Dictionary<string, string>
            {
                {"%(StylesheetIncludes)", ""},
                {"%(DiscoveryPaths)", string.Join("|", discoveryPaths ?? new string[] {})},
                {"%(BooleanValues)", "true|false"},
                {"%(ValidatorUrl)", ""},
                {"%(CustomScripts)", ""},
                {"%(DocExpansion)", "none"},
                {"%(OAuth2Enabled)", "false"},
                {"%(OAuth2ClientId)", ""},
                {"%(OAuth2ClientSecret)", ""},
                {"%(OAuth2Realm)", ""},
                {"%(OAuth2AppName)", ""}
            };
            _rootUrlResolver = rootUrlResolver;

            MapPathsForSwaggerUiAssets();

            // Use some custom versions to support config and extensionless paths
            var thisAssembly = GetType().Assembly;
            CustomAsset("index", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.index.html");
            CustomAsset("css/screen-css", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.screen.css");
            CustomAsset("css/typography-css", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.typography.css");
            CustomAsset("lib/swagger-oauth-js", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.swagger-oauth.js");
        }

        public void InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen")
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(StylesheetIncludes)"]);
            stringBuilder.AppendLine("<link href='" + path + "' media='" + media +
                                     "' rel='stylesheet' type='text/css' />");
            _templateParams["%(StylesheetIncludes)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName);
        }

        public void BooleanValues(IEnumerable<string> values)
        {
            _templateParams["%(BooleanValues)"] = string.Join("|", values);
        }

        public void SetValidatorUrl(string url)
        {
            _templateParams["%(ValidatorUrl)"] = url;
        }

        public void DisableValidator()
        {
            _templateParams["%(ValidatorUrl)"] = "null";
        }

        public void InjectJavaScript(Assembly resourceAssembly, string resourceName)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(CustomScripts)"]);
            if (stringBuilder.Length > 0)
                stringBuilder.Append("|");

            stringBuilder.Append(path);
            _templateParams["%(CustomScripts)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName);
        }

        public void DocExpansion(DocExpansion docExpansion)
        {
            _templateParams["%(DocExpansion)"] = docExpansion.ToString().ToLower();
        }

        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName)
        {
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, path == "index");
        }

        public void EnableDiscoveryUrlSelector()
        {
            InjectJavaScript(GetType().Assembly, "Abp.SwaggerUi.CustomAssets.discoveryUrlSelector.js");
        }

        public void EnableOAuth2Support(string clientId, string realm, string appName)
        {
            EnableOAuth2Support(clientId, "N/A", realm, appName);
        }

        public void EnableOAuth2Support(string clientId, string clientSecret, string realm, string appName)
        {
            _templateParams["%(OAuth2Enabled)"] = "true";
            _templateParams["%(OAuth2ClientId)"] = clientId;
            _templateParams["%(OAuth2ClientSecret)"] = clientSecret;
            _templateParams["%(OAuth2Realm)"] = realm;
            _templateParams["%(OAuth2AppName)"] = appName;
        }

        internal IAssetProvider GetSwaggerUiProvider()
        {
            return new EmbeddedAssetProvider(_pathToAssetMap, _templateParams);
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        private void MapPathsForSwaggerUiAssets()
        {
            var thisAssembly = GetType().Assembly;
            foreach (var resourceName in thisAssembly.GetManifestResourceNames())
            {
                if (resourceName.Contains("Abp.SwaggerUi.CustomAssets")) continue; // original assets only

                var path = resourceName
                    .Replace("\\", "/")
                    .Replace(".", "-"); // extensionless to avoid RUMMFAR

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }
    }

    public enum DocExpansion
    {
        None,
        List,
        Full
    }
}
