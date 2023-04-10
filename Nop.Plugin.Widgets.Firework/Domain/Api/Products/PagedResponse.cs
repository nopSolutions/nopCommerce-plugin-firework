using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Products
{
    public class PagedResponse<T> : ApiResponse
    {
        [JsonProperty("page_number")]
        public int PageNumber { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_entries")]
        public int TotalEntries { get; set; }

        [JsonProperty("entries")]
        public IList<T> Entries { get; set; }
    }
}