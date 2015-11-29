using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NSwag;

namespace Abp.NSwagExtended
{
    public class SwaggerOperationExtended : SwaggerOperation
    {
        public SwaggerOperationExtended()
        {
            Parameters = new List<SwaggerParameterExtended>();
        }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new List<SwaggerParameterExtended> Parameters { get; set; }

        /// <summary>Gets the parent operations list.</summary>
        [JsonIgnore]
        public new SwaggerOperationsExtended Parent { get; internal set; }

        /// <summary>Gets the parameters from the operation and from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public new IEnumerable<SwaggerParameterExtended> AllParameters
        {
            get
            {
                var empty = new List<SwaggerParameterExtended>();
                return (Parameters ?? empty).Concat(Parent.Parameters ?? empty).Concat(Parent.Parent.Parameters ?? empty);
            }
        }
    }
}
