using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents base request to Business GraphQL Mutation
    /// </summary>
    public abstract class BusinessApiRequest : ApiRequest, IAuthorizedRequest, IBodiedRequest
    {
        /// <summary>
        /// Gets or sets the Business identifier
        /// </summary>
        [JsonIgnore]
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        [JsonIgnore]
        public abstract string Token { get; set; }

        /// <summary>
        /// Gets the request Body
        /// </summary>
        [JsonIgnore]
        public abstract string Body { get; }
    }
}