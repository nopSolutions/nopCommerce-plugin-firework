using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api
{
    /// <summary>
    /// Represents base response details
    /// </summary>
    public class ApiResponse : IApiResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public string Error { get; set; }
    }
}