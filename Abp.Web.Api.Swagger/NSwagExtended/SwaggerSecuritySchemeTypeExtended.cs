using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Abp.NSwagExtended
{
    public enum SwaggerSecuritySchemeTypeExtended
    {
        /// <summary>The security scheme is not defined.</summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>Basic authentication.</summary>
        [JsonProperty("basic")]
        [EnumMember(Value = "basic")]
        Basic,

        /// <summary>API key authentication.</summary>
        [JsonProperty("apiKey")]
        [EnumMember(Value = "apiKey")]
        ApiKey,

        /// <summary>OAuth2 authentication.</summary>
        [JsonProperty("oauth2")]
        [EnumMember(Value = "oauth2")]
        OAuth2
    }
}
