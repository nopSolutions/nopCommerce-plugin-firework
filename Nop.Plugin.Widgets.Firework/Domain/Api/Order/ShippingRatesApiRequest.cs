using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework shipping rates API request
    /// </summary>
    public class ShippingRatesApiRequest
    {
        /// <summary>
        /// Gets or sets the shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public FireworkAddress ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        [JsonProperty("business_id")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the business store identifier
        /// </summary>
        [JsonProperty("business_store_id")]
        public string BusinessStoreId { get; set; }

        /// <summary>
        /// Gets or sets the cart items
        /// </summary>
        [JsonProperty("cart_items")]
        public List<FireworkCartItem> CartItems { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonProperty("metadata")]
        public FireworkCartMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
