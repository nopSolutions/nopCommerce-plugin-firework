using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Firework.Models
{
    public record EmbedWidgetModel : BaseNopEntityModel, IStoreMappingSupportedModel
    {
        #region Ctor

        public EmbedWidgetModel()
        {
            AvailableChannels = new List<SelectListItem>();
            AvailablePlaylists = new List<SelectListItem>();
            AvailableVideos = new List<SelectListItem>();
            AvailableWidgetZones = new List<SelectListItem>();
            AvailableLayoutTypes = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailablePlacements = new List<SelectListItem>();
            AvailablePlayerPlacements = new List<SelectListItem>();

        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Channel")]
        public string ChannelId { get; set; }
        public IList<SelectListItem> AvailableChannels { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist")]
        public string PlaylistId { get; set; }
        public IList<SelectListItem> AvailablePlaylists { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Video")]
        public string VideoId { get; set; }
        public IList<SelectListItem> AvailableVideos { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.WidgetZone")]
        public int WidgetZoneId { get; set; }
        public string WidgetZoneValue { get; set; }
        public IList<SelectListItem> AvailableWidgetZones { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.LayoutType")]
        public int LayoutTypeId { get; set; }
        public IList<SelectListItem> AvailableLayoutTypes { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.SelectedStoreIds")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Loop")]
        public bool Loop { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.AutoPlay")]
        public bool AutoPlay { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.MaxVideos")]
        public int MaxVideos { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Placement")]
        public string Placement { get; set; }
        public IList<SelectListItem> AvailablePlacements { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.PlayerPlacement")]
        public string PlayerPlacement { get; set; }
        public IList<SelectListItem> AvailablePlayerPlacements { get; set; }

        #endregion
    }
}