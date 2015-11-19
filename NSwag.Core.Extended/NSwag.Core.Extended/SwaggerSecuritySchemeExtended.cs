using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    public class SwaggerSecuritySchemeExtended : SwaggerSecurityScheme
    {
        /// <summary>Gets or sets the type of the security scheme.</summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public new SwaggerSecuritySchemeTypeExtended Type { get; set; }
    }
}
