using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework payment option
    /// </summary>
    public class FireworkPaymentOption
    {
        /// <summary>
        /// Gets or sets the payment option identifier
        /// </summary>
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets the payment option details
        /// </summary>
        [JsonProperty("payment_details")]
        public PaymentDetails PaymentDetails { get; set; }
    }

    /// <summary>
    /// Represents a Firework payment details
    /// </summary>
    public class PaymentDetails
    {
        /// <summary>
        /// Gets or sets the payment provider
        /// </summary>
        [JsonProperty("payment_provider")]
        public string PaymentProvider { get; set; }

        /// <summary>
        /// Gets or sets the payment reference
        /// </summary>
        [JsonProperty("payment_reference")]
        public string PaymentReference { get; set; }

        /// <summary>
        /// Gets or sets the payment detail URL
        /// </summary>
        [JsonProperty("payment_detail_url")]
        public string PaymentDetailUrl { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        [JsonProperty("firework_order_id")]
        public string FireworkOrderId { get; set; }
    }
}
