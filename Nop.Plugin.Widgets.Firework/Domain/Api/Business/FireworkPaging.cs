using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents a base pagination response
    /// </summary>
    public class FireworkPaging
    {
        /// <summary>
        /// API request URL for next page
        /// </summary>
        [JsonProperty("next")]
        public string Next { get; set; }

        /// <summary>
        /// API request URL for previous page
        /// </summary>
        [JsonProperty("prev")]
        public string Prev { get; set; }
    }
}
