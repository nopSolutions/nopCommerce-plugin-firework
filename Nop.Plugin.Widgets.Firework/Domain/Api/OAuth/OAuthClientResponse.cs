using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.OAuth
{
    /// <summary>
    /// Represents oauth client details
    /// </summary>
    public class OAuthClientResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client application
        /// </summary>
        [JsonProperty(PropertyName = "client_name")]
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the client secret
        /// </summary>
        [JsonProperty(PropertyName = "client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the list of contact emails
        /// </summary>
        [JsonProperty(PropertyName = "contacts")]
        public string[] Contacts { get; set; }

        /// <summary>
        /// Gets or sets the firework oauth app identifier
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string OAuthAppId { get; set; }

        /// <summary>
        /// Gets or sets the list of redirect URIs
        /// </summary>
        [JsonProperty(PropertyName = "redirect_uris")]
        public string[] RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the space-separated list of scopes
        /// </summary>
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
    }
}
