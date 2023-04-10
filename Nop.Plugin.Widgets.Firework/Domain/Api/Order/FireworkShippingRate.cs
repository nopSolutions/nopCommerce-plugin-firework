using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework shipping rate
    /// </summary>
    public class FireworkShippingRate
    {
        /// <summary>
        /// Gets or sets the shipping rate identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the shipping rate name
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the shipping rate price
        /// </summary>
        [JsonProperty("cost")]
        public string Cost { get; set; }
    }
}
