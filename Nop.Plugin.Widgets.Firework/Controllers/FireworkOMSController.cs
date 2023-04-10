using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Firework.Services;
using Nop.Services.Cms;

namespace Nop.Plugin.Widgets.Firework.Controllers
{
    public class FireworkOMSController : Controller
    {
        #region Fields

        private readonly FireworkService _fireworkService;
        private readonly FireworkSettings _fireworkSettings;
        private readonly IWidgetPluginManager _widgetPluginManager;

        #endregion

        #region Ctor

        public FireworkOMSController(FireworkService fireworkService,
            FireworkSettings fireworkSettings,
            IWidgetPluginManager widgetPluginManager)
        {
            _fireworkService = fireworkService;
            _fireworkSettings = fireworkSettings;
            _widgetPluginManager = widgetPluginManager;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string businessId, string businessStoreId,
            [FromQuery] string search = null,
            [FromQuery] int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 10)
        {
            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return BadRequest("Not enabled");

            if (!FireworkService.IsConfigured(_fireworkSettings) || !FireworkService.IsConnected(_fireworkSettings))
                return BadRequest("Not configured");

            if (businessId != _fireworkSettings.BusinessId || businessStoreId != _fireworkSettings.BusinessStoreId)
                return BadRequest("Wrong business");

            if (!await _fireworkService.ValidateRequestAsync(Request))
                return BadRequest("Validation failed");

            var (response, error) = await _fireworkService.GetImportProductsAsync(search, page, pageSize);
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductByExtId(string businessId, string businessStoreId, string productExtId)
        {
            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return BadRequest("Not enabled");

            if (!FireworkService.IsConfigured(_fireworkSettings) || !FireworkService.IsConnected(_fireworkSettings))
                return BadRequest("Not configured");

            if (businessId != _fireworkSettings.BusinessId || businessStoreId != _fireworkSettings.BusinessStoreId)
                return BadRequest("Wrong business");

            if (!await _fireworkService.ValidateRequestAsync(Request))
                return BadRequest("Validation failed");

            var (response, error) = await _fireworkService.GetImportProductAsync(productExtId);
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            return Json(response);
        }

        #endregion
    }
}