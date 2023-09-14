using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.Firework.Helpers;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.Firework
{
    /// <summary>
    /// Represents Firework plugin
    /// </summary>
    public class FireworkPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Ctor

        public FireworkPlugin(IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            var widgetZones = EmbedWidgetHelper.GetCustomWidgetZones();
            return Task.FromResult<IList<string>>(widgetZones);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(FireworkDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return FireworkDefaults.VIEW_COMPONENT;
        }

        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var systemItem = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            if (systemItem is null)
                return;

            var pluginNode = new SiteMapNode
            {
                SystemName = "Firework Portal",
                Title = await _localizationService.GetResourceAsync("Plugins.Widgets.Firework"),
                Visible = true,
                IconClass = "fa fa-infinity text-sm",
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            };

            pluginNode.ChildNodes.Add(new SiteMapNode
            {
                SystemName = "Plugins.Widgets.Firework.Menu.Configure",
                Title = await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.Menu.Configure"),
                ControllerName = "Firework",
                ActionName = "Configure",
                Visible = true,
                IconClass = "far fa-dot-circle",
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });

            pluginNode.ChildNodes.Add(new SiteMapNode
            {
                SystemName = "Plugins.Widgets.Firework.Menu.Portal",
                Title = await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.Menu.Portal"),
                ControllerName = "Firework",
                ActionName = "Portal",
                Visible = true,
                IconClass = "far fa-dot-circle",
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });

            rootNode.ChildNodes.Insert(rootNode.ChildNodes.IndexOf(systemItem) + 1, pluginNode);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new FireworkSettings
            {
                RequestTimeout = FireworkDefaults.RequestTimeout
            });

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.Firework"] = "Firework",
                ["Plugins.Widgets.Firework.Menu.Configure"] = "Configuration",
                ["Plugins.Widgets.Firework.Menu.Portal"] = "Portal",
                ["Plugins.Widgets.Firework.Portal"] = "Portal",

                ["Plugins.Widgets.Firework.Configuration"] = "Configuration",
                ["Plugins.Widgets.Firework.Configuration.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
                ["Plugins.Widgets.Firework.Configuration.Fields.BusinessId"] = "Firework business ID",
                ["Plugins.Widgets.Firework.Configuration.Fields.BusinessId.Hint"] = "The firework business id contains firework business identity (read only).",
                ["Plugins.Widgets.Firework.Configuration.Fields.BusinessStoreId"] = "Business store ID",
                ["Plugins.Widgets.Firework.Configuration.Fields.BusinessStoreId.Hint"] = "Business store identifier received from Firework",
                ["Plugins.Widgets.Firework.Configuration.Fields.ClientId"] = "Client ID",
                ["Plugins.Widgets.Firework.Configuration.Fields.ClientId.Hint"] = "The client id is used together with the access token to identify the firwork bussiness id (read only).",
                ["Plugins.Widgets.Firework.Configuration.Fields.ClientSecret"] = "Client secret",
                ["Plugins.Widgets.Firework.Configuration.Fields.ClientSecret.Hint"] = "The client secret helps to generate access token. It is used together with the access token to identify the firwork bussiness id (read only).",
                ["Plugins.Widgets.Firework.Configuration.Fields.ClientSecret.Required"] = "Client secret is required",
                ["Plugins.Widgets.Firework.Configuration.Fields.Connected"] = "Firework status",
                ["Plugins.Widgets.Firework.Configuration.Fields.Connected.Hint"] = "Whether the plugin is connected with the fireworks business or not.",
                ["Plugins.Widgets.Firework.Configuration.Fields.Email"] = "Email",
                ["Plugins.Widgets.Firework.Configuration.Fields.Email.Hint"] = "Enter your email to get access to Firework services.",
                ["Plugins.Widgets.Firework.Configuration.Fields.UseSandbox"] = "Use staging",
                ["Plugins.Widgets.Firework.Configuration.Fields.UseSandbox.Hint"] = "Whether to use staging environment.",

                ["Plugins.Widgets.Firework.EmbedWidget"] = "Embed widget",
                ["Plugins.Widgets.Firework.EmbedWidget.Added"] = "The widget has been added successfully",
                ["Plugins.Widgets.Firework.EmbedWidget.AddNew"] = "Add new widget",
                ["Plugins.Widgets.Firework.EmbedWidget.BackToList"] = "back to list",
                ["Plugins.Widgets.Firework.EmbedWidget.Deleted"] = "The widget has been deleted successfully",
                ["Plugins.Widgets.Firework.EmbedWidget.Edit"] = "Edit embed widget",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Active"] = "Active",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Active.Hint"] = "Check to activate the widget",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.AutoPlay"] = "Autoplay",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.AutoPlay.Hint"] = "Enable autoplay",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Channel"] = "Channel ID",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Channel.Hint"] = "Firework channel ID to use",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Channel.Required"] = "Channel ID is required",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.CreatedOn"] = "Created on",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.CreatedOn.Hint"] = "Created on",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.DisplayOrder"] = "Display order",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.DisplayOrder.Hint"] = "Order of display",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.LayoutType"] = "Layout type",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.LayoutType.Hint"] = "Type of layout",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Loop"] = "Loop",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Loop.Hint"] = "Check to loop the video",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.MaxVideos"] = "Max no. of videos",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.MaxVideos.Hint"] = "Maximum number of videos to show",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Placement"] = "Placement",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Placement.Hint"] = "Position the component will be shown",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.PlayerPlacement"] = "Player placement",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.PlayerPlacement.Hint"] = "Position of the player",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist"] = "Playlist ID",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist.Hint"] = "Firework playlist ID to use",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist.Required"] = "Playlist ID is required",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.SelectedStoreIds"] = "Stores",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.SelectedStoreIds.Hint"] = "The stores the widget will be shown",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Title"] = "Title",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Title.Hint"] = "Name of the widget",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.UpdatedOn"] = "Updated on",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.UpdatedOn.Hint"] = "Updated on",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Video"] = "Video ID",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.Video.Hint"] = "Firework Video ID to use",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.WidgetZone"] = "Widget zone",
                ["Plugins.Widgets.Firework.EmbedWidget.Fields.WidgetZone.Hint"] = "Widget zone to show",
                ["Plugins.Widgets.Firework.EmbedWidget.PlayerPlacement.BottomLeft"] = "Bottom Left",
                ["Plugins.Widgets.Firework.EmbedWidget.PlayerPlacement.BottomRight"] = "Bottom Right",
                ["Plugins.Widgets.Firework.EmbedWidget.PlayerPlacement.TopLeft"] = "Top Left",
                ["Plugins.Widgets.Firework.EmbedWidget.PlayerPlacement.TopRight"] = "Top Right",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.Active"] = "Status",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.Active.Hint"] = "Search by active status",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.LayoutType"] = "Layout type",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.LayoutType.Hint"] = "Search by layout type",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.Store"] = "Store",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.Store.Hint"] = "Search by store",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.WidgetZone"] = "Widget zone",
                ["Plugins.Widgets.Firework.EmbedWidget.Search.WidgetZone.Hint"] = "Search by widget zone",
                ["Plugins.Widgets.Firework.EmbedWidget.Updated"] = "The widget has been updated successfully",

                ["Plugins.Widgets.Firework.OAuth"] = "Credentials",
                ["Plugins.Widgets.Firework.OAuth.Button"] = "Connect Firework",
                ["Plugins.Widgets.Firework.OAuth.ButtonReset"] = "Reset credentials",
                ["Plugins.Widgets.Firework.OAuth.Connected"] = "Connected",
                ["Plugins.Widgets.Firework.OAuth.Disconnected"] = "Disconnected",

                ["Plugins.Widgets.Firework.ShoppingCart.ItemRemoved"] = "The product has been removed from your shopping cart",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<FireworkSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.Firework");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}