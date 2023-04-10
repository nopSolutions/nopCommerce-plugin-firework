using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.Firework.Domain;
using Nop.Plugin.Widgets.Firework.Helpers;
using Nop.Plugin.Widgets.Firework.Models;
using Nop.Plugin.Widgets.Firework.Services;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.Firework.Factories
{
    /// <summary>
    /// Represents widget model factory
    /// </summary>
    public class FireworkModelFactory
    {
        #region Fields

        private readonly FireworkService _fireworkService;
        private readonly FireworkWidgetService _fireworkWidgetService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public FireworkModelFactory(FireworkService fireworkService,
            FireworkWidgetService fireworkWidgetService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            IStoreMappingService storeMappingService)
        {
            _fireworkService = fireworkService;
            _fireworkWidgetService = fireworkWidgetService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Utilities

        private async Task PreparePlayerPlacementsAsync(IList<SelectListItem> selectListItems)
        {
            selectListItems ??= new List<SelectListItem>();

            var positions = new[] { "Bottom Right", "Bottom Left", "Top Right", "Top Left" };

            foreach (var position in positions)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync($"Plugins.Widgets.Firework.EmbedWidget.PlayerPlacement.{position.Replace(" ", "")}"),
                    Value = position.Replace(' ', '-').ToLower()
                });
            }
        }

        #endregion

        #region Methods

        public async Task<EmbedWidgetModel> PrepareEmbedWidgetModelAsync(EmbedWidgetModel model, FireworkEmbedWidget embedWidget, bool excludeProperties = false)
        {
            if (embedWidget is not null)
            {
                model ??= embedWidget.ToModel<EmbedWidgetModel>();
                model.Title = await _localizationService.GetLocalizedAsync(embedWidget, w => w.Title);
                model.WidgetZoneValue = EmbedWidgetHelper.GetCustomWidgetZoneNameValues().FirstOrDefault(x => x.Value == embedWidget.WidgetZoneId)?.Name;
                model.SelectedStoreIds = (await _storeMappingService.GetStoresIdsWithAccessAsync(embedWidget)).ToList();
                if (!model.SelectedStoreIds.Any())
                    model.SelectedStoreIds = new List<int> { 0 };
            }

            if (!excludeProperties)
            {
                await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
                model.AvailableLayoutTypes = (await LayoutType.HeroUnit.ToSelectListAsync(false)).ToList();
                model.AvailableWidgetZones = EmbedWidgetHelper.GetCustomWidgetZoneSelectList();
                await PreparePlayerPlacementsAsync(model.AvailablePlayerPlacements);

                model.AvailableChannels.Add(new SelectListItem(await _localizationService.GetResourceAsync("Admin.Common.Select"), string.Empty));
                var (channels, _) = await _fireworkService.GetChannelsAsync();
                foreach (var channel in channels)
                {
                    model.AvailableChannels.Add(new SelectListItem($"{channel.Name} ({channel.Id})", channel.Id));
                }

                if (!string.IsNullOrEmpty(model.ChannelId))
                {
                    var (playlists, _) = await _fireworkService.GetPlaylistsAsync(model.ChannelId);
                    foreach (var playlist in playlists)
                    {
                        model.AvailablePlaylists.Add(new SelectListItem($"{playlist.Name} ({playlist.Id})", playlist.Id));
                    }
                }
                else
                    model.AvailablePlaylists.Add(new SelectListItem(await _localizationService.GetResourceAsync("Admin.Common.Select"), string.Empty));

                if (!string.IsNullOrEmpty(model.ChannelId) && !string.IsNullOrEmpty(model.PlaylistId))
                {
                    var (videos, _) = await _fireworkService.GetVideosAsync(model.ChannelId, model.PlaylistId);
                    foreach (var video in videos)
                    {
                        model.AvailableVideos.Add(new SelectListItem($"{video.Caption} ({video.EncodedId})", video.EncodedId));
                    }
                }
                else
                    model.AvailableVideos.Add(new SelectListItem(await _localizationService.GetResourceAsync("Admin.Common.Select"), string.Empty));
            }

            return model;
        }

        public async Task<EmbedWidgetSearchModel> PrepareEmbedWidgetSearchModelAsync(EmbedWidgetSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<EmbedWidgetListModel> PrepareEmbedWidgetListModelAsync(EmbedWidgetSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            var embedWidgets = await _fireworkWidgetService.GetEmbedWidgetsAsync(widgetZoneId: searchModel.SearchWidgetZoneId,
                activeOnly: false,
                storeId: searchModel.SearchStoreId,
                layoutType: searchModel.SearchLayoutTypeId == 0 ? null : (LayoutType)searchModel.SearchLayoutTypeId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = await new EmbedWidgetListModel().PrepareToGridAsync(searchModel, embedWidgets, () =>
            {
                return embedWidgets.SelectAwait(async embedWidget => await PrepareEmbedWidgetModelAsync(null, embedWidget, true));
            });

            return model;
        }

        #endregion
    }
}