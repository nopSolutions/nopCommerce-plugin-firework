using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents response to get playlists request
    /// </summary>
    public class GetPlaylistsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the playlists
        /// </summary>
        [JsonProperty("playlists")]
        public List<Playlist> Playlists { get; set; }

        /// <summary>
        /// Gets or sets the pagination
        /// </summary>
        [JsonProperty("paging")]
        public FireworkPaging Paging { get; set; }

    }
}