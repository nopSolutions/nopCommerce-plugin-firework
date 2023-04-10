using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents request to get videos
    /// </summary>
    public class GetVideosRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        [JsonIgnore]
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the channel identifier
        /// </summary>
        [JsonIgnore]
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the playlist identifier
        /// </summary>
        [JsonIgnore]
        public string PlaylistId { get; set; }

        public override string Path => $"api/bus/{BusinessId}/channels/{ChannelId}/playlists/{PlaylistId}/videos";

        public override string Method => HttpMethods.Get;

        [JsonIgnore]
        public string Token { get; set; }
    }
}