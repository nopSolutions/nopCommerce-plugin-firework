using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.OAuth
{
    /// <summary>
    /// Represents request to register oauth application
    /// </summary>
    public class RegisterRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the name of the client application
        /// </summary>
        [JsonProperty(PropertyName = "client_name")]
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the list of redirect URIs
        /// </summary>
        [JsonProperty(PropertyName = "redirect_uris")]
        public string[] RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the list of contact emails
        /// </summary>
        [JsonProperty(PropertyName = "contacts")]
        public string[] Contacts { get; set; }

        /// <summary>
        /// Gets or sets the space-separated list of scopes
        /// </summary>
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "oauth/register";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}
