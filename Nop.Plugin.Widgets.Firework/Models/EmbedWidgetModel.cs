using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Firework.Models
{
    public record EmbedWidgetModel : BaseNopEntityModel, ILocalizedModel<EmbedWidgetLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public EmbedWidgetModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
            AvailableLayoutTypes = new List<SelectListItem>();
            AvailablePlayerPlacements = new List<SelectListItem>();
            AvailableChannels = new List<SelectListItem>();
            AvailablePlaylists = new List<SelectListItem>();
            AvailableVideos = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            Locales = new List<EmbedWidgetLocalizedModel>();

        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.WidgetZone")]
        public int WidgetZoneId { get; set; }
        public string WidgetZoneValue { get; set; }
        public IList<SelectListItem> AvailableWidgetZones { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.LayoutType")]
        public int LayoutTypeId { get; set; }
        public IList<SelectListItem> AvailableLayoutTypes { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Channel")]
        public string ChannelId { get; set; }
        public IList<SelectListItem> AvailableChannels { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist")]
        public string PlaylistId { get; set; }
        public IList<SelectListItem> AvailablePlaylists { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Video")]
        public string VideoId { get; set; }
        public IList<SelectListItem> AvailableVideos { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Loop")]
        public bool Loop { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.AutoPlay")]
        public bool AutoPlay { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.MaxVideos")]
        public int MaxVideos { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Placement")]
        public string Placement { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.PlayerPlacement")]
        public string PlayerPlacement { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.SelectedStoreIds")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailablePlayerPlacements { get; set; }

        public IList<EmbedWidgetLocalizedModel> Locales { get; set; }

        #endregion
    }

    public class EmbedWidgetLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Fields.Title")]
        public string Title { get; set; }
    }
}