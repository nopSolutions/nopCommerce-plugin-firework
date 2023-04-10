using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents request to get channels
    /// </summary>
    public class GetChannelsRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        [JsonIgnore]
        public string BusinessId { get; set; }

        public override string Path => $"api/bus/{BusinessId}/channels";

        public override string Method => HttpMethods.Get;

        [JsonIgnore]
        public string Token { get; set; }
    }
}
