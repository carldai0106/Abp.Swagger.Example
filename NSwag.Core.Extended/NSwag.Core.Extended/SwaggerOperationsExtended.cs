using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag.Collections;

namespace NSwag
{
    public class SwaggerOperationsExtended : ObservableDictionary<SwaggerOperationMethod, SwaggerOperationExtended>
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperations"/> class.</summary>
        public SwaggerOperationsExtended()
        {
            CollectionChanged += (sender, args) =>
            {
                foreach (var operation in Values)
                    operation.Parent = this; 
            };
        }

        /// <summary>Gets the parent <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public SwaggerServiceExtended Parent { get; internal set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerParameterExtended> Parameters { get; set; }
    }
}
