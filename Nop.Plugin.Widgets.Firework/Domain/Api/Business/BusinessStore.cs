using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    public class BusinessStore
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("business_id")]
        public string BusinessId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("provider_details")]
        public object ProviderDetails { get; set; }

        [JsonProperty("provider_metadata")]
        public object ProviderMetadata { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("payment_settings")]
        public PaymentSettings PaymentSetting { get; set; }

        public class PaymentSettings
        {
            [JsonProperty("fast_enabled")]
            public bool FastEnabled { get; set; }

            [JsonProperty("fast_app_id")]
            public object FastAppId { get; set; }
        }
    }
}
