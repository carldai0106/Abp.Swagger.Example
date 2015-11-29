using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Abp.NSwagExtended
{
    public enum SwaggerParameterKindExtended
    {
        /// <summary>An undefined kind.</summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>A JSON object as POST or PUT body (only one parameter of this type is allowed). </summary>
        [JsonProperty("body")]
        [EnumMember(Value = "body")]
        Body,

        /// <summary>A query key-value pair. </summary>
        [JsonProperty("query")]
        [EnumMember(Value = "query")]
        Query,

        /// <summary>An URL path placeholder. </summary>
        [JsonProperty("path")]
        [EnumMember(Value = "path")]
        Path,

        /// <summary>A HTTP header parameter.</summary>
        [JsonProperty("header")]
        [EnumMember(Value = "header")]
        Header,

        /// <summary>A form data parameter.</summary>
        [JsonProperty("formData")]
        [EnumMember(Value = "formData")]
        FormData
    }
}
