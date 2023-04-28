namespace Nop.Plugin.Widgets.Firework.Domain
{
    /// <summary>
    /// Represents widget layout type
    /// </summary>
    public enum LayoutType
    {
        /// <summary>
        /// Storyblock is used when you want to integrate the video player directly on to your website
        /// </summary>
        Storyblock,

        /// <summary>
        /// Floating player is a storyblock that is pinned on a specific location on the page
        /// </summary>
        FloatingPlayer,

        /// <summary>
        /// Channel Button allows you to integrate the video feed in a circular
        /// </summary>
        //ChannelButton, //deprecated

        /// <summary>
        /// Embed feed allows the content to be displayed as a row (carousel) or grid of thumbnails
        /// </summary>
        Grid,

        /// <summary>
        /// Embed feed allows the content to be displayed as a row (carousel) or grid of thumbnails
        /// </summary>
        Carousel,

        /// <summary>
        /// Hero Unit allows you livestream event to be promoted with countdown, sharing, and add to calendar
        /// </summary>
        HeroUnit
    }
}