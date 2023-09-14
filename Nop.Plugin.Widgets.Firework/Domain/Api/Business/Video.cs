using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents a Firework Video
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        [JsonProperty("caption")]
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the encoded id
        /// </summary>
        [JsonProperty("encoded_id")]
        public string EncodedId { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL
        /// </summary>
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the video type
        /// </summary>
        [JsonProperty("video_type")]
        public string VideoType { get; set; }
    }
}
