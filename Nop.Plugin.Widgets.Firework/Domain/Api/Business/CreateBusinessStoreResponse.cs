using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents response to create new business store request
    /// </summary>
    public class CreateBusinessStoreResponse : BusinessApiResponse
    {
        [JsonProperty("data")]
        public CreateBusinessStoreResult Data { get; set; }

        public class CreateBusinessStoreResult
        {
            [JsonProperty("createBusinessStore")]
            public CreateBusinessStoreResultData Result { get; set; }

            public class CreateBusinessStoreResultData
            {
                [JsonProperty("accessToken")]
                public string AccessToken { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("currency")]
                public string Currency { get; set; }

                [JsonProperty("provider")]
                public string Provider { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }
            }
        }
    }
}