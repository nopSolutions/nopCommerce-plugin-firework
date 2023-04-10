using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Business
{
    /// <summary>
    /// Represents a Firework playlist
    /// </summary>
    public class Playlist
    {
        /// <summary>
        /// Gets or sets the playlist description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the playlist display name
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the playlist identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the playlist name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the playlist thumbnail URL
        /// </summary>
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the playlist updated time
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the playlist video identifiers
        /// </summary>
        [JsonProperty("video_ids")]
        public List<string> VideoIds { get; set; }

        /// <summary>
        /// Gets or sets the playlist videos URL
        /// </summary>
        [JsonProperty("videos_url")]
        public string VideosUrl { get; set; }
    }
}
