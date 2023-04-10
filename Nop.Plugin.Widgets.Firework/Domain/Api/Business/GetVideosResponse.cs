using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents response to get videos request
    /// </summary>
    public class GetVideosResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the videos
        /// </summary>
        [JsonProperty("videos")]
        public List<Video> Videos { get; set; }

        /// <summary>
        /// Gets or sets the pagination
        /// </summary>
        [JsonProperty("paging")]
        public FireworkPaging Paging { get; set; }

    }
}