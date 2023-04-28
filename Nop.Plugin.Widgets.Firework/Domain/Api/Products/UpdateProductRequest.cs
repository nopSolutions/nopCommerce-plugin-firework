using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Products
{
    /// <summary>
    /// Represents request to update a product
    /// </summary>
    public class UpdateProductRequest : FireworkProduct, IApiRequest, IAuthorizedRequest, IWebhookRequest
    {
        /// <summary>
        /// Gets or sets the content signature
        /// </summary>
        [JsonIgnore]
        public string ContentSignature { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        [JsonIgnore]
        public string Token { get; set; }

        /// <summary>
        /// Gets the event type
        /// </summary>
        [JsonIgnore]
        public string EventType => "Update";

        [JsonIgnore]
        public string Method => HttpMethods.Post;

        [JsonIgnore]
        public string Path => $"webhooks/generic_oms/{BusinessStoreId}";

        public static UpdateProductRequest FromFireworkProduct(FireworkProduct fireworkProduct)
        {
            return new UpdateProductRequest
            {
                BusinessId = fireworkProduct.BusinessId,
                BusinessStoreId = fireworkProduct.BusinessStoreId,
                BusinessStoreName = fireworkProduct.BusinessStoreName,
                BusinessStoreUid = fireworkProduct.BusinessStoreUid,
                ProductCurrency = fireworkProduct.ProductCurrency,
                ProductDescription = fireworkProduct.ProductDescription,
                ProductExtId = fireworkProduct.ProductExtId,
                ProductHandle = fireworkProduct.ProductHandle,
                ProductImages = fireworkProduct.ProductImages,
                ProductName = fireworkProduct.ProductName,
                ProductOptions = fireworkProduct.ProductOptions,
                ProductUnits = fireworkProduct.ProductUnits
            };
        }
    }
}