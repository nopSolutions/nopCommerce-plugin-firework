using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.Firework.Domain;
using Nop.Plugin.Widgets.Firework.Factories;
using Nop.Plugin.Widgets.Firework.Models;
using Nop.Plugin.Widgets.Firework.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Firework.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class FireworkController : BasePluginController
    {
        #region Fields

        private readonly FireworkModelFactory _fireworkModelFactory;
        private readonly FireworkService _fireworkService;
        private readonly FireworkSettings _fireworkSettings;
        private readonly FireworkWidgetService _fireworkWidgetService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FireworkController(FireworkModelFactory fireworkModelFactory,
            FireworkService fireworkService,
            FireworkSettings fireworkSettings,
            FireworkWidgetService fireworkWidgetService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _fireworkModelFactory = fireworkModelFactory;
            _fireworkService = fireworkService;
            _fireworkSettings = fireworkSettings;
            _fireworkWidgetService = fireworkWidgetService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        #region Configuration

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                UseSandbox = _fireworkSettings.UseSandbox,
                Email = _fireworkSettings.Email,
                ClientId = _fireworkSettings.ClientId,
                ClientSecret = _fireworkSettings.ClientSecret,
                Configured = FireworkService.IsConfigured(_fireworkSettings),
                Connected = FireworkService.IsConnected(_fireworkSettings),
                BusinessId = _fireworkSettings.BusinessId,
                BusinessStoreId = _fireworkSettings.BusinessStoreId
            };

            if (!string.IsNullOrEmpty(_fireworkSettings.BusinessId))
            {
                var ((businessStores, selectedStoreId), _) = await _fireworkService.GetBusinessStoresAsync();

                model.AvailableBusinessStores = businessStores.Select(businessStore => new SelectListItem
                {
                    Text = $"{businessStore.Name} ({businessStore.Id})",
                    Value = businessStore.Id
                }).ToList();

                if (selectedStoreId != _fireworkSettings.BusinessStoreId)
                {
                    model.BusinessStoreId = selectedStoreId;
                    _fireworkSettings.BusinessStoreId = selectedStoreId;
                    await _settingService.SaveSettingAsync(_fireworkSettings);
                }

                if (!string.IsNullOrEmpty(_fireworkSettings.BusinessStoreId))
                {
                    //ensure HMAC is created and stored
                    await _fireworkService.GetHmacSecretAsync(_fireworkSettings);
                }
                else
                {
                    model.AvailableBusinessStores
                        .Insert(0, new SelectListItem(await _localizationService.GetResourceAsync("Admin.Common.Select"), string.Empty));
                }
            }

            model.EmbedWidgetSearchModel = await _fireworkModelFactory.PrepareEmbedWidgetSearchModelAsync(new());

            return View("~/Plugins/Widgets.Firework/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("credentials")]
        public async Task<IActionResult> SaveCredentials(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            _fireworkSettings.UseSandbox = model.UseSandbox;
            _fireworkSettings.BusinessStoreId = model.BusinessStoreId;

            if (!FireworkService.IsConfigured(_fireworkSettings))
            {
                _fireworkSettings.Email = model.Email;

                var (client, _) = await _fireworkService.RegisterOAuthAsync(model.Email);

                _fireworkSettings.ClientId = client.ClientId;
                _fireworkSettings.ClientSecret = client.ClientSecret;
                _fireworkSettings.OAuthAppId = client.OAuthAppId;
                _fireworkSettings.BusinessId = string.Empty;
                _fireworkSettings.RefreshToken = string.Empty;
                _fireworkSettings.AccessToken = string.Empty;
                _fireworkSettings.RefreshTokenExpiresIn = null;
                _fireworkSettings.TokenExpiresIn = null;
                _fireworkSettings.BusinessStoreId = string.Empty;
                _fireworkSettings.HmacSecret = string.Empty;
            }

            await _settingService.SaveSettingAsync(_fireworkSettings);

            if (!string.IsNullOrEmpty(_fireworkSettings.BusinessStoreId))
            {
                //ensure HMAC is created and stored
                await _fireworkService.GetHmacSecretAsync(_fireworkSettings);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> ConnectAccount()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!FireworkService.IsConfigured(_fireworkSettings) || FireworkService.IsConnected(_fireworkSettings))
                return BadRequest();

            var url = await _fireworkService.GetAuthorizationUrlAsync();

            return Redirect(url);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("reset-credentials")]
        public async Task<IActionResult> ResetCredentials(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            _fireworkSettings.Email = string.Empty;
            _fireworkSettings.ClientId = string.Empty;
            _fireworkSettings.ClientSecret = string.Empty;
            _fireworkSettings.OAuthAppId = string.Empty;
            _fireworkSettings.RefreshToken = string.Empty;
            _fireworkSettings.AccessToken = string.Empty;
            _fireworkSettings.TokenExpiresIn = null;
            _fireworkSettings.RefreshTokenExpiresIn = null;
            _fireworkSettings.BusinessId = string.Empty;
            _fireworkSettings.BusinessStoreId = string.Empty;
            _fireworkSettings.HmacSecret = string.Empty;

            await _settingService.SaveSettingAsync(_fireworkSettings);

            await _fireworkWidgetService.ClearEmbedWidgetsAsync();

            return RedirectToAction(nameof(Configure));
        }

        public async Task<IActionResult> OauthCallback()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var (result, _) = await _fireworkService.HandleOauthCallbackAsync(Request);
            ViewBag.RefreshPage = result;

            //select default store
            if (result && !string.IsNullOrEmpty(_fireworkSettings.BusinessId))
            {
                var ((_, selectedStoreId), _) = await _fireworkService.GetBusinessStoresAsync();
                if (selectedStoreId != _fireworkSettings.BusinessStoreId)
                {
                    _fireworkSettings.BusinessStoreId = selectedStoreId;
                    await _settingService.SaveSettingAsync(_fireworkSettings);
                }

                if (!string.IsNullOrEmpty(_fireworkSettings.BusinessStoreId))
                {
                    //ensure HMAC is created and stored
                    await _fireworkService.GetHmacSecretAsync(_fireworkSettings);
                }
            }

            return View("~/Plugins/Widgets.Firework/Views/OauthCallback.cshtml");
        }

        #endregion

        #region Business portal

        public async Task<IActionResult> Portal()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var (token, _) = await _fireworkService.GetAccessTokenAsync();

            var model = new PortalModel()
            {
                BusinessId = _fireworkSettings.BusinessId,
                BusinessStoreId = _fireworkSettings.BusinessStoreId,
                AccessToken = token
            };

            return View("~/Plugins/Widgets.Firework/Views/Portal.cshtml", model);
        }

        #endregion

        #region Embed widgets

        public async Task<IActionResult> Playlists(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
                return BadRequest("Channel ID is empty");

            var (playlists, _) = await _fireworkService.GetPlaylistsAsync(channelId);

            return Json(playlists.Select(playlist => new { playlist.Id, playlist.Name }));
        }

        public async Task<IActionResult> Videos(string channelId, string playlistId, int layoutTypeId)
        {
            if (string.IsNullOrEmpty(channelId))
                return BadRequest("Channel ID is empty");

            if (string.IsNullOrEmpty(playlistId))
                return BadRequest("Playlist ID is empty");

            var (videos, _) = await _fireworkService.GetVideosAsync(channelId, playlistId);

            if (layoutTypeId == (int)LayoutType.HeroUnit)
                videos = videos.Where(video => string.Equals(video.VideoType, "live_stream", StringComparison.InvariantCultureIgnoreCase)).ToList();

            return Json(videos.Select(video => new { video.EncodedId, video.Caption }));
        }

        [HttpPost]
        public async Task<IActionResult> ListEmbedWidget(EmbedWidgetSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return await AccessDeniedDataTablesJson();

            var model = await _fireworkModelFactory.PrepareEmbedWidgetListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> CreateEmbedWidget()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var model = await _fireworkModelFactory.PrepareEmbedWidgetModelAsync(new(), null);
            model.Active = true;

            return View("~/Plugins/Widgets.Firework/Views/CreateEmbedWidget.cshtml", model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> CreateEmbedWidget(EmbedWidgetModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var embedWidget = new FireworkEmbedWidget
                {
                    Active = model.Active,
                    ChannelId = model.ChannelId,
                    PlaylistId = model.PlaylistId,
                    VideoId = model.VideoId,
                    WidgetZoneId = model.WidgetZoneId,
                    LayoutTypeId = model.LayoutTypeId,
                    Title = model.Title,
                    DisplayOrder = model.DisplayOrder,
                    LimitedToStores = model.SelectedStoreIds.Any(),
                    Loop = model.Loop,
                    AutoPlay = model.AutoPlay,
                    MaxVideos = model.MaxVideos,
                    Placement = model.Placement,
                    PlayerPlacement = model.PlayerPlacement,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _fireworkWidgetService.InsertEmbedWidgetAsync(embedWidget);

                await _fireworkWidgetService.UpdateEmbedWidgetStoreMappingsAsync(embedWidget, model.SelectedStoreIds);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.EmbedWidget.Added"));

                return continueEditing ? RedirectToAction(nameof(EditEmbedWidget), new { id = embedWidget.Id }) : RedirectToAction(nameof(Configure));
            }

            model = await _fireworkModelFactory.PrepareEmbedWidgetModelAsync(model, null, true);
            return View("~/Plugins/Widgets.Firework/Views/CreateEmbedWidget.cshtml", model);
        }

        public async Task<IActionResult> EditEmbedWidget(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var embedWidget = await _fireworkWidgetService.GetEmbedWidgetByIdAsync(id);
            if (embedWidget is null)
                return RedirectToAction(nameof(Configure));

            var model = await _fireworkModelFactory.PrepareEmbedWidgetModelAsync(null, embedWidget);

            return View("~/Plugins/Widgets.Firework/Views/EditEmbedWidget.cshtml", model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> EditEmbedWidget(EmbedWidgetModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var embedWidget = await _fireworkWidgetService.GetEmbedWidgetByIdAsync(model.Id);
            if (embedWidget is null)
                return RedirectToAction(nameof(Configure));

            if (ModelState.IsValid)
            {
                embedWidget.Active = model.Active;
                embedWidget.ChannelId = model.ChannelId;
                embedWidget.PlaylistId = model.PlaylistId;
                embedWidget.VideoId = model.VideoId;
                embedWidget.WidgetZoneId = model.WidgetZoneId;
                embedWidget.LayoutTypeId = model.LayoutTypeId;
                embedWidget.Title = model.Title;
                embedWidget.DisplayOrder = model.DisplayOrder;
                embedWidget.LimitedToStores = model.SelectedStoreIds.Any();
                embedWidget.Loop = model.Loop;
                embedWidget.AutoPlay = model.AutoPlay;
                embedWidget.MaxVideos = model.MaxVideos;
                embedWidget.Placement = model.Placement;
                embedWidget.PlayerPlacement = model.PlayerPlacement;

                await _fireworkWidgetService.UpdateEmbedWidgetAsync(embedWidget);

                await _fireworkWidgetService.UpdateEmbedWidgetStoreMappingsAsync(embedWidget, model.SelectedStoreIds);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.EmbedWidget.Updated"));

                return continueEditing ? RedirectToAction(nameof(EditEmbedWidget), new { id = embedWidget.Id }) : RedirectToAction(nameof(Configure));
            }

            model = await _fireworkModelFactory.PrepareEmbedWidgetModelAsync(model, embedWidget, true);

            return View("~/Plugins/Widgets.Firework/Views/EditEmbedWidget.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmbedWidget(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return await AccessDeniedDataTablesJson();

            var embedWidget = await _fireworkWidgetService.GetEmbedWidgetByIdAsync(id)
                ?? throw new ArgumentException("No widget found with the specified id", nameof(id));

            await _fireworkWidgetService.DeleteEmbedWidgetAsync(embedWidget);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var embedWidget = await _fireworkWidgetService.GetEmbedWidgetByIdAsync(id);
            if (embedWidget is null)
                return RedirectToAction(nameof(Configure));

            await _fireworkWidgetService.DeleteEmbedWidgetAsync(embedWidget);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.EmbedWidget.Deleted"));

            return RedirectToAction(nameof(Configure));
        }

        #endregion

        #endregion
    }
}