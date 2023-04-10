using System;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Widgets.Firework.Domain
{
    /// <summary>
    /// Represents a Firework embed widget
    /// </summary>
    public class FireworkEmbedWidget : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        /// <summary>
        /// Gets or sets wether the widget is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the widget title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the widget zone identifier
        /// </summary>
        public int WidgetZoneId { get; set; }

        /// <summary>
        /// Gets or sets the layout type identifier
        /// </summary>
        public int LayoutTypeId { get; set; }

        /// <summary>
        /// Gets or sets the channel identifier
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the playlist identifier
        /// </summary>
        public string PlaylistId { get; set; }

        /// <summary>
        /// Gets or sets the video identifier
        /// </summary>
        public string VideoId { get; set; }

        /// <summary>
        /// Gets or sets the widget is looped
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets the widget is autoplayed
        /// </summary>
        public bool AutoPlay { get; set; }

        /// <summary>
        /// Gets or sets the widget maximum video count
        /// </summary>
        public int MaxVideos { get; set; }

        /// <summary>
        /// Gets or sets the widget placement
        /// </summary>
        public string Placement { get; set; }

        /// <summary>
        /// Gets or sets the widget video player placement
        /// </summary>
        public string PlayerPlacement { get; set; }

        /// <summary>
        /// Gets or sets the widget display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the widget creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets if the widget is limited to stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the layout type
        /// </summary>
        public LayoutType LayoutType
        {
            get => (LayoutType)LayoutTypeId;
            set => LayoutTypeId = (int)value;
        }
    }
}