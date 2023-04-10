using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    public class HmacApiRequest : BusinessApiRequest
    {
        public override string Method => HttpMethods.Post;

        [JsonIgnore]
        public override string Token { get; set; }

        public string BusinessStoreId { get; set; }

        public override string Path => $"/graphiql";

        public override string Body => $@"mutation {{
             businessStoreShuffleHmacSecret(businessStoreId: ""{BusinessStoreId}""){{
             ... on BusinessStoreShuffleHmacSecretResult {{hmacSecret}}
             ... on AnyError {{message}}
             }} 
            }}
        ";
    }
}
