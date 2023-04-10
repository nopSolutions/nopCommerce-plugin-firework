using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents base response to request to Business GraphQL Mutation
    /// </summary>
    public class BusinessApiResponse : IApiResponse
    {
        [JsonProperty(PropertyName = "errors")]
        public List<string> Errors { get; set; }

        public string Error { get; set; }
    }
}