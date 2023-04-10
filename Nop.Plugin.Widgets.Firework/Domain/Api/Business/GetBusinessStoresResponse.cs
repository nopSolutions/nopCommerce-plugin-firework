using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents response to get business stores request
    /// </summary>
    public class GetBusinessStoresResponse : ApiResponse
    {
        public GetBusinessStoresResponse()
        {
            BusinessStores = new List<BusinessStore>();
        }

        [JsonProperty("business_stores")]
        public List<BusinessStore> BusinessStores { get; set; }

        [JsonProperty("paging")]
        public FireworkPaging Paging { get; set; }
    }
}
