using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework shipping rates API response
    /// </summary>
    public class ShippingRatesApiResponse
    {
        public ShippingRatesApiResponse()
        {
            ShippingRates = new List<FireworkShippingRate>();
            CartItems = new List<FireworkCartItem>();
        }

        /// <summary>
        /// Gets or sets the subtotal
        /// </summary>
        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        [JsonProperty("total")]
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the shipping cost
        /// </summary>
        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        /// <summary>
        /// Gets or sets the tax
        /// </summary>
        [JsonProperty("tax")]
        public string Tax { get; set; }

        /// <summary>
        /// Gets or sets the discount
        /// </summary>
        [JsonProperty("discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the shipping rates
        /// </summary>
        [JsonProperty("shipping_rates")]
        public List<FireworkShippingRate> ShippingRates { get; set; }

        /// <summary>
        /// Gets or sets the cart items
        /// </summary>
        [JsonProperty("cart_items")]
        public List<FireworkCartItem> CartItems { get; set; }

        /// <summary>
        /// Gets or sets the cart metadata
        /// </summary>
        [JsonProperty("metadata")]
        public FireworkCartMetadata Metadata { get; set; }
    }
}