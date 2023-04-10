using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework place order API request
    /// </summary>
    public class PlaceOrderApiRequest
    {
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
        /// Gets or sets timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        [JsonProperty("receipt_email")]
        public string ReceiptEmail { get; set; }

        /// <summary>
        /// Gets or sets the payment option
        /// </summary>
        [JsonProperty("payment")]
        public FireworkPaymentOption Payment { get; set; }

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

    }
}
