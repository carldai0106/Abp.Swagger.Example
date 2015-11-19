using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Collections;

namespace NSwag
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
        public string ToJson(JsonConverter converter = null)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };

            if (converter != null)
            {
                settings.Converters.Add(converter);
            }

            GenerateOperationIds();

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            var data = JsonConvert.SerializeObject(this, settings);
            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }
    }
}
