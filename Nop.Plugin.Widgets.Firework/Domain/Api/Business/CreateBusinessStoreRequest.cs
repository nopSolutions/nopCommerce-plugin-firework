using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents request to create new business store
    /// </summary>
    public class CreateBusinessStoreRequest : BusinessApiRequest
    {
        public override string Method => HttpMethods.Post;

        [JsonIgnore]
        public override string Token { get; set; }

        public string Name { get; set; }

        public string Currency { get; set; }

        public string Uid { get; set; }

        public string Url { get; set; }

        public string ApiUrl { get; set; }

        public override string Path => $"/graphiql";

        public override string Body => $@"mutation {{
            createBusinessStore(createBusinessStoreInput:{{
                businessId: ""{BusinessId}"", currency:""{Currency}"", name:""{Name}"", provider: ""{FireworkDefaults.ProviderName}"", uid:""{Uid}"", url:""{Url}"", hostApiUrl:""{ApiUrl}"" }}) {{
                ... on BusinessStore {{
                    id
                    name
                    provider
                    currency
                    url
                    accessToken
                }} 
                ... on AnyError {{
                    message
                }}
            }} 
        }}
        ";
    }
}