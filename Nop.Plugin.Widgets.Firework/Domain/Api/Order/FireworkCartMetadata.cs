using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework cart metadata
    /// </summary>
    public class FireworkCartMetadata
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [JsonProperty("customer_guid")]
        public string CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        [JsonProperty("order_id")]
        public string OrderGuid { get; set; }
    }
}