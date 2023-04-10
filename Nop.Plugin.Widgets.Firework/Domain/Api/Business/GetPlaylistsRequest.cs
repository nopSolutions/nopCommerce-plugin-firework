using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents request to get playlists
    /// </summary>
    public class GetPlaylistsRequest : ApiRequest, IAuthorizedRequest
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

        public override string Path => $"api/bus/{BusinessId}/channels/{ChannelId}/playlists";

        public override string Method => HttpMethods.Get;

        [JsonIgnore]
        public string Token { get; set; }
    }
}