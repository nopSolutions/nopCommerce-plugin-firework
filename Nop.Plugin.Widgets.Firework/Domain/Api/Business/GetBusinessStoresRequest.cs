using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents request to get business stores
    /// </summary>
    public class GetBusinessStoresRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        [JsonIgnore]
        public string BusinessId { get; set; }

        [JsonIgnore]
        public int PageSize { get; set; }

        public override string Path => $"api/bus/{BusinessId}/business_stores{(PageSize > 0 ? $"?page_size={PageSize}" : string.Empty)}";

        public override string Method => HttpMethods.Get;

        [JsonIgnore]
        public string Token { get; set; }
    }
}
