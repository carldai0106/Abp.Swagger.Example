using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag;

namespace Abp.NSwagExtended
{
    public class SwaggerSecuritySchemeExtended : SwaggerSecurityScheme
    {
        /// <summary>Gets or sets the type of the security scheme.</summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public new SwaggerSecuritySchemeTypeExtended Type { get; set; }
    }
}
