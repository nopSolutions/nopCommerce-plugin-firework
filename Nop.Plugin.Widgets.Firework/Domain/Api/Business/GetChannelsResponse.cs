using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents response to get channels request
    /// </summary>
    public class GetChannelsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the channels
        /// </summary>
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }

        /// <summary>
        /// Gets or sets the pagination
        /// </summary>
        [JsonProperty("paging")]
        public FireworkPaging Paging { get; set; }
    }
}