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
using Nop.Web.Framework.Factories;
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
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public FireworkModelFactory(FireworkService fireworkService,
            FireworkWidgetService fireworkWidgetService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _fireworkService = fireworkService;
            _fireworkWidgetService = fireworkWidgetService;
            _localizationService = localizationService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
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

        public async Task<EmbedWidgetModel> PrepareEmbedWidgetModelAsync(EmbedWidgetModel model, FireworkEmbedWidget embedWidget, bool excludeProperties = false)
        {
            if (embedWidget is not null)
            {
                model ??= new()
                {
                    Id = embedWidget.Id,
                    Active = embedWidget.Active,
                    ChannelId = embedWidget.ChannelId,
                    PlaylistId = embedWidget.PlaylistId,
                    VideoId = embedWidget.VideoId,
                    LayoutTypeId = embedWidget.LayoutTypeId,
                    WidgetZoneId = embedWidget.WidgetZoneId,
                    Title = embedWidget.Title,
                    DisplayOrder = embedWidget.DisplayOrder,
                    Loop = embedWidget.Loop,
                    AutoPlay = embedWidget.AutoPlay,
                    MaxVideos = embedWidget.MaxVideos,
                    Placement = embedWidget.Placement,
                    PlayerPlacement = embedWidget.PlayerPlacement,
                    WidgetZoneValue = EmbedWidgetHelper.GetCustomWidgetZoneNameValues().FirstOrDefault(x => x.Value == embedWidget.WidgetZoneId)?.Name
                };
            }

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, embedWidget, excludeProperties);

            if (!excludeProperties)
            {
                model.AvailableLayoutTypes = (await LayoutType.HeroUnit.ToSelectListAsync(false)).ToList();
                model.AvailableWidgetZones = EmbedWidgetHelper.GetCustomWidgetZoneSelectList();
                await PreparePlayerPlacementsAsync(model.AvailablePlayerPlacements);
                model.AvailablePlacements = new[] { "Top", "Middle", "Bottom" }
                    .Select(placement => new SelectListItem(placement, placement))
                    .ToList();

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

                    if (model.LayoutTypeId == (int)LayoutType.HeroUnit)
                        videos = videos.Where(video => string.Equals(video.VideoType, "live_stream", StringComparison.InvariantCultureIgnoreCase)).ToList();

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

        #endregion
    }
}