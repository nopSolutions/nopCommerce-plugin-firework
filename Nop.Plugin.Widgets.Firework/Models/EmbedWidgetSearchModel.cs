using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Firework.Models
{
    public record EmbedWidgetSearchModel : BaseSearchModel
    {
        #region Ctor

        public EmbedWidgetSearchModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
            AvailableLayoutTypes = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableActiveOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.List.WidgetZone")]
        public int SearchWidgetZoneId { get; set; }
        public IList<SelectListItem> AvailableWidgetZones { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.List.LayoutType")]
        public int SearchLayoutTypeId { get; set; }
        public IList<SelectListItem> AvailableLayoutTypes { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Search.Store")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.EmbedWidget.Search.Active")]
        public int SearchActiveId { get; set; }
        public IList<SelectListItem> AvailableActiveOptions { get; set; }

        #endregion
    }
}