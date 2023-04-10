using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    public class PaymentBreakdownApiResponse
    {
        public PaymentBreakdownApiResponse()
        {
            CartItems = new List<FireworkCartItem>();
            DiscountApplications = new List<DiscountApplication>();
        }

        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }

        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("discount_applications")]
        public List<DiscountApplication> DiscountApplications { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("cart_items")]
        public List<FireworkCartItem> CartItems { get; set; }

        [JsonProperty("metadata")]
        public FireworkCartMetadata Metadata { get; set; }
    }

    public class DiscountApplication
    {
        [JsonProperty("discount_code")]
        public string DiscountCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("percentage")]
        public string Percentage { get; set; }
    }
}
