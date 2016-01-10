using System.Collections.Generic;
using Abp.Builders;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.Collections;

namespace Abp.NSwagExtended
{
    public class SwaggerServiceExtended : SwaggerService
    {
        public SwaggerServiceExtended()
        {
            Tags = new List<SwaggerTagInfo>();
            SecurityDefinitions = new Dictionary<string, SwaggerSecuritySchemeExtended>();

            Paths = new ObservableDictionary<string, SwaggerOperationsExtended>();
            Paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };
        }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new List<SwaggerTagInfo> Tags { get; set; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securityDefinitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new Dictionary<string, SwaggerSecuritySchemeExtended> SecurityDefinitions { get; private set; }

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new ObservableDictionary<string, SwaggerOperationsExtended> Paths { get; private set; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new List<SwaggerParameterExtended> Parameters { get; set; }


        /// <summary>Converts the description object to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public new string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };

            var swaggerOperationsConverter = new SwaggerOperationsExtendedConverter(typeof(SwaggerOperationsExtended));
            var jsonSchema4Converter = new JsonSchema4Converter(typeof (JsonSchema4));

            settings.Converters.Add(swaggerOperationsConverter);
            settings.Converters.Add(jsonSchema4Converter);

            GenerateOperationIds();

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            var data = JsonConvert.SerializeObject(this, settings);

            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }
    }
}
