using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Firework.Factories;
using Nop.Plugin.Widgets.Firework.Helpers;
using Nop.Plugin.Widgets.Firework.Services;
using Nop.Services.Cms;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Firework.Components
{
    /// <summary>
    /// Represents Firework embedded wideget view component
    /// </summary>
    [ViewComponent(Name = FireworkDefaults.VIEW_COMPONENT)]
    public class FireworkWidgetViewComponent : NopViewComponent
    {
        #region Fields

        private readonly FireworkModelFactory _fireworkModelFactory;
        private readonly FireworkWidgetService _fireworkWidgetService;
        private readonly IStoreContext _storeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FireworkWidgetViewComponent(FireworkModelFactory fireworkModelFactory,
            FireworkWidgetService fireworkWidgetService,
            IStoreContext storeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _fireworkModelFactory = fireworkModelFactory;
            _fireworkWidgetService = fireworkWidgetService;
            _storeContext = storeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that plugin is active for the current customer and the store, since it's the event from the public area
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName, customer, store.Id))
                return Content(string.Empty);

            if (!EmbedWidgetHelper.TryGetWidgetZoneId(widgetZone, out var widgetZoneId))
                return Content(string.Empty);

            var widgets = await _fireworkWidgetService.GetEmbedWidgetsAsync(widgetZoneId: widgetZoneId, storeId: store.Id);
            var model = await widgets
                .SelectAwait(async widget => await _fireworkModelFactory.PrepareEmbedWidgetModelAsync(null, widget, true))
                .ToListAsync();

            return View("~/Plugins/Widgets.Firework/Views/FireworkWidget.cshtml", model);
        }

        #endregion
    }
}