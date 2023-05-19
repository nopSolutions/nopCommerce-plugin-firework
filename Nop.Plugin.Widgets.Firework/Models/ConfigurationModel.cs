using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Firework.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            EmbedWidgetSearchModel = new EmbedWidgetSearchModel();
            AvailableBusinessStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool Configured { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.Connected")]
        public bool Connected { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.BusinessId")]
        public string BusinessId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Firework.Configuration.Fields.BusinessStoreId")]
        public string BusinessStoreId { get; set; }

        public IList<SelectListItem> AvailableBusinessStores { get; set; }

        public EmbedWidgetSearchModel EmbedWidgetSearchModel { get; set; }

        #endregion
    }
}