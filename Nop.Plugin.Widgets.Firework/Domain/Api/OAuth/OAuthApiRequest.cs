using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.OAuth
{
    /// <summary>
    /// Represents request to get access token
    /// </summary>
    public class OAuthApiRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the grant type
        /// </summary>
        [JsonIgnore]
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI
        /// </summary>
        [JsonIgnore]
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the client ID
        /// </summary>
        [JsonIgnore]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret
        /// </summary>
        [JsonIgnore]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the authorization code
        /// </summary>
        [JsonIgnore]
        public string AuthCode { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        [JsonIgnore]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"oauth/token?grant_type={GrantType}&redirect_uri={RedirectUri}&client_id={ClientId}&client_secret={ClientSecret}&code={AuthCode}&refresh_token={RefreshToken}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}