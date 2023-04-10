using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.OAuth
{
    /// <summary>
    /// Represents a update response object
    /// </summary>
    public class UpdateProductResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }
    }
}