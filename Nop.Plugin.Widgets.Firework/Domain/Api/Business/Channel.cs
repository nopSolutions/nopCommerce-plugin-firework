using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    public class Channel
    {
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("business_id")]
        public string BusinessId { get; set; }

        [JsonProperty("config")]
        public ChannelConfig Config { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("encoded_id")]
        public string EncodedId { get; set; }

        [JsonProperty("ga_tracking_id")]
        public string GaTrackingId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("promote_url")]
        public string PromoteUrl { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        public class ChannelConfig
        {
            [JsonProperty("autoplay")]
            public bool Autoplay { get; set; }

            [JsonProperty("base_tag_ids")]
            public string BaseTagIds { get; set; }

            [JsonProperty("captions")]
            public bool Captions { get; set; }

            [JsonProperty("creator_ids")]
            public string CreatorIds { get; set; }

            [JsonProperty("cta_disabled")]
            public object CtaDisabled { get; set; }

            [JsonProperty("dashboard_id")]
            public string DashboardId { get; set; }

            [JsonProperty("dashboard_settings")]
            public object DashboardSettings { get; set; }

            [JsonProperty("email_embed")]
            public bool EmailEmbed { get; set; }

            [JsonProperty("hashtags")]
            public string Hashtags { get; set; }

            [JsonProperty("max_videos")]
            public int MaxVideos { get; set; }

            [JsonProperty("mode")]
            public string Mode { get; set; }

            [JsonProperty("open_in")]
            public string OpenIn { get; set; }

            [JsonProperty("pip")]
            public string Pip { get; set; }

            [JsonProperty("pixel")]
            public object Pixel { get; set; }

            [JsonProperty("placement")]
            public string Placement { get; set; }

            [JsonProperty("restrict_by_content_bucket")]
            public bool RestrictByContentBucket { get; set; }

            [JsonProperty("router")]
            public string Router { get; set; }

            [JsonProperty("share")]
            public string Share { get; set; }

            [JsonProperty("share_template")]
            public string ShareTemplate { get; set; }

            [JsonProperty("size")]
            public string Size { get; set; }

            [JsonProperty("ui_border_style")]
            public string UiBorderStyle { get; set; }

            [JsonProperty("ui_button_color")]
            public string UiButtonColor { get; set; }

            [JsonProperty("ui_button_font_color")]
            public string UiButtonFontColor { get; set; }
        }
    }
}
