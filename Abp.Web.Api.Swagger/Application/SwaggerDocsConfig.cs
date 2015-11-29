using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Abp.Swagger;
using Abp.Swagger.Annotations;
using Abp.Swagger.FromUriParams;
using Abp.Swagger.XmlComments;

namespace Abp.Application
{
    public class SwaggerDocsConfig
    {
        private VersionInfoBuilder _versionInfoBuilder;
        private Func<ApiDescription, string, bool> _versionSupportResolver;
        private IEnumerable<string> _schemes;
        private readonly IDictionary<string, SecuritySchemeBuilder> _securitySchemeBuilders;
        private bool _ignoreObsoleteActions;
        private Func<ApiDescription, string> _groupingKeySelector;
        private IComparer<string> _groupingKeyComparer;
        private readonly IDictionary<Type, Func<Schema>> _customSchemaMappings;
        private readonly IList<Func<ISchemaFilter>> _schemaFilters;
        private readonly IList<Func<IModelFilter>> _modelFilters;
        private bool _ignoreObsoleteProperties;
        private Func<Type, string> _schemaIdSelector;
        private bool _describeAllEnumsAsStrings;
        private bool _describeStringEnumsInCamelCase;
        private readonly IList<Func<IOperationFilter>> _operationFilters;
        private readonly IList<Func<IDocumentFilter>> _documentFilters;
        private Func<IEnumerable<ApiDescription>, ApiDescription> _conflictingActionsResolver;
        private Func<HttpRequestMessage, string> _rootUrlResolver;

        private Func<ISwaggerProvider, ISwaggerProvider> _customProviderFactory;

        public SwaggerDocsConfig()
        {
            _versionInfoBuilder = new VersionInfoBuilder();
            _securitySchemeBuilders = new Dictionary<string, SecuritySchemeBuilder>();
            _ignoreObsoleteActions = false;
            _customSchemaMappings = new Dictionary<Type, Func<Schema>>();
            _schemaFilters = new List<Func<ISchemaFilter>>();
            _modelFilters = new List<Func<IModelFilter>>();
            _ignoreObsoleteProperties = false;
            _describeAllEnumsAsStrings = false;
            _describeStringEnumsInCamelCase = false;
            _operationFilters = new List<Func<IOperationFilter>>();
            _documentFilters = new List<Func<IDocumentFilter>>();
            _rootUrlResolver = DefaultRootUrlResolver;

            SchemaFilter<ApplySwaggerSchemaFilterAttributes>();

            OperationFilter<HandleFromUriParams>();
            OperationFilter<ApplySwaggerOperationAttributes>();
            OperationFilter<ApplySwaggerResponseAttributes>();
            OperationFilter<ApplySwaggerOperationFilterAttributes>();
        }

        public InfoBuilder SingleApiVersion(string version, string title)
        {
            _versionSupportResolver = null;
            _versionInfoBuilder = new VersionInfoBuilder();
            return _versionInfoBuilder.Version(version, title);
        }

        public void MultipleApiVersions(
            Func<ApiDescription, string, bool> versionSupportResolver,
            Action<VersionInfoBuilder> configure)
        {
            _versionSupportResolver = versionSupportResolver;
            _versionInfoBuilder = new VersionInfoBuilder();
            configure(_versionInfoBuilder);
        }

        public void Schemes(IEnumerable<string> schemes)
        {
            _schemes = schemes;
        }

        public BasicAuthSchemeBuilder BasicAuth(string name)
        {
            var schemeBuilder = new BasicAuthSchemeBuilder();
            _securitySchemeBuilders[name] = schemeBuilder;
            return schemeBuilder;
        }

        public ApiKeySchemeBuilder ApiKey(string name)
        {
            var schemeBuilder = new ApiKeySchemeBuilder();
            _securitySchemeBuilders[name] = schemeBuilder;
            return schemeBuilder;
        }

        public OAuth2SchemeBuilder OAuth2(string name)
        {
            var schemeBuilder = new OAuth2SchemeBuilder();
            _securitySchemeBuilders[name] = schemeBuilder;
            return schemeBuilder;
        }

        public void IgnoreObsoleteActions()
        {
            _ignoreObsoleteActions = true;
        }

        public void GroupActionsBy(Func<ApiDescription, string> keySelector)
        {
            _groupingKeySelector = keySelector;
        }

        public void OrderActionGroupsBy(IComparer<string> keyComparer)
        {
            _groupingKeyComparer = keyComparer;
        }

        public void MapType<T>(Func<Schema> factory)
        {
            _customSchemaMappings.Add(typeof (T), factory);
        }

        public void SchemaFilter<TFilter>()
            where TFilter : ISchemaFilter, new()
        {
            SchemaFilter(() => new TFilter());
        }

        public void SchemaFilter(Func<ISchemaFilter> factory)
        {
            _schemaFilters.Add(factory);
        }

        // NOTE: In next major version, ModelFilter will completely replace SchemaFilter
        internal void ModelFilter<TFilter>()
            where TFilter : IModelFilter, new()
        {
            ModelFilter(() => new TFilter());
        }

        // NOTE: In next major version, ModelFilter will completely replace SchemaFilter
        internal void ModelFilter(Func<IModelFilter> factory)
        {
            _modelFilters.Add(factory);
        }

        public void SchemaId(Func<Type, string> schemaIdStrategy)
        {
            _schemaIdSelector = schemaIdStrategy;
        }

        public void UseFullTypeNameInSchemaIds()
        {
            _schemaIdSelector = t => t.FriendlyId(true);
        }

        public void DescribeAllEnumsAsStrings(bool camelCase = false)
        {
            _describeAllEnumsAsStrings = true;
            _describeStringEnumsInCamelCase = camelCase;
        }

        public void IgnoreObsoleteProperties()
        {
            _ignoreObsoleteProperties = true;
        }

        public void OperationFilter<TFilter>()
            where TFilter : IOperationFilter, new()
        {
            OperationFilter(() => new TFilter());
        }

        public void OperationFilter(Func<IOperationFilter> factory)
        {
            _operationFilters.Add(factory);
        }

        public void DocumentFilter<TFilter>()
            where TFilter : IDocumentFilter, new()
        {
            DocumentFilter(() => new TFilter());
        }

        public void DocumentFilter(Func<IDocumentFilter> factory)
        {
            _documentFilters.Add(factory);
        }

        public void IncludeXmlComments(string filePath)
        {
            OperationFilter(() => new ApplyXmlActionComments(filePath));
            ModelFilter(() => new ApplyXmlTypeComments(filePath));
        }

        public void ResolveConflictingActions(
            Func<IEnumerable<ApiDescription>, ApiDescription> conflictingActionsResolver)
        {
            _conflictingActionsResolver = conflictingActionsResolver;
        }

        public void RootUrl(Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _rootUrlResolver = rootUrlResolver;
        }

        public void CustomProvider(Func<ISwaggerProvider, ISwaggerProvider> customProviderFactory)
        {
            _customProviderFactory = customProviderFactory;
        }

        internal ISwaggerProvider GetSwaggerProvider(HttpRequestMessage swaggerRequest)
        {
            var httpConfig = swaggerRequest.GetConfiguration();

            var securityDefintitions = _securitySchemeBuilders.Any()
                ? _securitySchemeBuilders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Build())
                : null;

            var options = new SwaggerGeneratorOptions(_versionSupportResolver, _schemes, securityDefintitions,
                _ignoreObsoleteActions, _groupingKeySelector, _groupingKeyComparer, _customSchemaMappings,
                _schemaFilters.Select(factory => factory()), _modelFilters.Select(factory => factory()),
                _ignoreObsoleteProperties, _schemaIdSelector, _describeAllEnumsAsStrings,
                _describeStringEnumsInCamelCase, _operationFilters.Select(factory => factory()),
                _documentFilters.Select(factory => factory()), _conflictingActionsResolver
                );

            var defaultProvider = new SwaggerGenerator(
                httpConfig.Services.GetApiExplorer(),
                httpConfig.SerializerSettingsOrDefault(),
                _versionInfoBuilder.Build(),
                options);

            return (_customProviderFactory != null)
                ? _customProviderFactory(defaultProvider)
                : defaultProvider;
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        internal IEnumerable<string> GetApiVersions()
        {
            return _versionInfoBuilder.Build().Select(entry => entry.Key);
        }

        public static string DefaultRootUrlResolver(HttpRequestMessage request)
        {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            var host = GetHeaderValue(request, "X-Forwarded-Host") ?? request.RequestUri.Host;
            var port = GetHeaderValue(request, "X-Forwarded-Port") ??
                       request.RequestUri.Port.ToString(CultureInfo.InvariantCulture);

            var httpConfiguration = request.GetConfiguration();
            var virtualPathRoot = httpConfiguration.VirtualPathRoot.TrimEnd('/');

            return string.Format("{0}://{1}:{2}{3}", scheme, host, port, virtualPathRoot);
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }
    }
}