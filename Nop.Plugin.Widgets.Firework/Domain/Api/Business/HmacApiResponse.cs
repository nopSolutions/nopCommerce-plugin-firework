using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    public class HmacApiResponse : BusinessApiResponse
    {
        [JsonProperty("data")]
        public BusinessStoreShuffleHmacSecretResult Data { get; set; }

        public class BusinessStoreShuffleHmacSecretResult
        {
            [JsonProperty("businessStoreShuffleHmacSecret")]
            public BusinessStoreShuffleHmacSecretData BusinessStoreShuffleHmacSecret { get; set; }

            public class BusinessStoreShuffleHmacSecretData
            {
                [JsonProperty("hmacSecret")]
                public string HmacSecret { get; set; }
            }
        }
    }
}
