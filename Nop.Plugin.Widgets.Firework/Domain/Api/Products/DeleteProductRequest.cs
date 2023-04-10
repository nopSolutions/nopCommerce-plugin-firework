using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Products
{
    /// <summary>
    /// Represents request to delete a product
    /// </summary>
    public class DeleteProductRequest : ApiRequest, IAuthorizedRequest, IWebhookRequest
    {
        /// <summary>
        /// Gets or sets the product ext identifier
        /// </summary>
        [JsonProperty("product_ext_id")]
        public string ProductExtId { get; set; }

        /// <summary>
        /// Gets or sets the content signature
        /// </summary>
        [JsonIgnore]
        public string ContentSignature { get; set; }

        /// <summary>
        /// Gets or sets the Business store identifier
        /// </summary>
        [JsonIgnore]
        public string BusinessStoreId { get; set; }

        /// <summary>
        /// Gets or sets access token
        /// </summary>
        [JsonIgnore]
        public string Token { get; set; }

        /// <summary>
        /// Gets the event type
        /// </summary>
        [JsonIgnore]
        public string EventType => "delete";

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public override string Path => $"webhooks/generic_oms/{BusinessStoreId}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}