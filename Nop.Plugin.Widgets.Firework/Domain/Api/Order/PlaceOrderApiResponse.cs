using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework place order API request
    /// </summary>
    public class PlaceOrderApiResponse
    {
        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        [JsonProperty("external_order_id")]
        public string ExternalOrderId { get; set; }

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
        /// Gets or sets the cart subtotal
        /// </summary>
        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the cart total
        /// </summary>
        [JsonProperty("total")]
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        [JsonProperty("discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the shipping
        /// </summary>
        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        /// <summary>
        /// Gets or sets the tax
        /// </summary>
        [JsonProperty("tax")]
        public string Tax { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

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

    }
}
