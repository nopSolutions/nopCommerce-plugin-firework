using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Widgets.Firework.Domain;
using Nop.Services.Catalog;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.Firework.Services
{
    /// <summary>
    /// Represents the service to manage widgets
    /// </summary>
    public class FireworkWidgetService
    {
        #region Fields

        private readonly IRepository<FireworkEmbedWidget> _embedWidgetRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public FireworkWidgetService(IRepository<FireworkEmbedWidget> embedWidgetRepository,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _embedWidgetRepository = embedWidgetRepository;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get embed widgets
        /// </summary>
        /// <param name="widgetZoneId">Search by widget zone identifier</param>
        /// <param name="activeOnly">Search only active items</param>
        /// <param name="storeId">Search by store identifier</param>
        /// <param name="layoutType">Search by layout type</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Per page item size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains widgets
        /// </returns>
        public async Task<IPagedList<FireworkEmbedWidget>> GetEmbedWidgetsAsync(int widgetZoneId = 0,
            bool activeOnly = true,
            int storeId = 0,
            LayoutType? layoutType = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _embedWidgetRepository.Table;

            if (widgetZoneId > 0)
                query = query.Where(ew => ew.WidgetZoneId == widgetZoneId);

            if (activeOnly)
                query = query.Where(ew => ew.Active);

            if (layoutType.HasValue)
                query = query.Where(ew => ew.LayoutTypeId == (int)layoutType.Value);

            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            query = query.OrderBy(ew => ew.DisplayOrder).ThenBy(ew => ew.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Get embed widget by id
        /// </summary>
        /// <param name="embedWidgetId">Embed widget ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the embed widget
        /// </returns>
        public async Task<FireworkEmbedWidget> GetEmbedWidgetByIdAsync(int embedWidgetId)
        {
            return await _embedWidgetRepository.GetByIdAsync(embedWidgetId, cache => default);
        }

        /// <summary>
        /// Insert embed widget
        /// </summary>
        /// <param name="embedWidget">Embed widget to add</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertEmbedWidgetAsync(FireworkEmbedWidget embedWidget)
        {
            await _embedWidgetRepository.InsertAsync(embedWidget, false);
        }

        /// <summary>
        /// Update embed widget
        /// </summary>
        /// <param name="embedWidget">Embed widget to update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateEmbedWidgetAsync(FireworkEmbedWidget embedWidget)
        {
            await _embedWidgetRepository.UpdateAsync(embedWidget, false);
        }

        /// <summary>
        /// Delete embed widget
        /// </summary>
        /// <param name="embedWidget">Embed widget to delete</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteEmbedWidgetAsync(FireworkEmbedWidget embedWidget)
        {
            await _embedWidgetRepository.DeleteAsync(embedWidget, false);
        }

        /// <summary>
        /// Clear embed widgets
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ClearEmbedWidgetsAsync()
        {
            await _embedWidgetRepository.TruncateAsync();
        }

        /// <summary>
        /// Update embed widget store mappings
        /// </summary>
        /// <param name="embedWidget">Entity to update store mappings for</param>
        /// <param name="limitedToStoresIds">Updated store identifiers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateEmbedWidgetStoreMappingsAsync(FireworkEmbedWidget embedWidget, IList<int> limitedToStoresIds)
        {
            embedWidget.LimitedToStores = limitedToStoresIds.Any();
            await UpdateEmbedWidgetAsync(embedWidget);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(embedWidget);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(embedWidget, store.Id);
                }
                else
                {
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete is not null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion
    }
}