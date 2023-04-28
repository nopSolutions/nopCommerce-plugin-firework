using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Widgets.Firework.Domain.Api.Order;
using Nop.Plugin.Widgets.Firework.Models;
using Nop.Plugin.Widgets.Firework.Services;
using Nop.Services.Cms;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Firework.Controllers
{
    public class FireworkPublicController : BasePluginController
    {
        #region Fields

        private readonly FireworkService _fireworkService;
        private readonly FireworkSettings _fireworkSettings;
        private readonly IWidgetPluginManager _widgetPluginManager;

        #endregion

        #region Ctor

        public FireworkPublicController(FireworkService fireworkService,
            FireworkSettings fireworkSettings,
            IWidgetPluginManager widgetPluginManager)
        {
            _fireworkService = fireworkService;
            _fireworkSettings = fireworkSettings;
            _widgetPluginManager = widgetPluginManager;
        }

        #endregion

        #region Methods

        #region Native checkout

        public async Task<IActionResult> ProductHydrate(string productExtId)
        {
            var (response, error) = await _fireworkService.GetProductsByExtIdsAsync(new() { productExtId });
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null || !response.Any(product => product is not null))
                return NotFound();

            return Json(response.FirstOrDefault());
        }

        public async Task<IActionResult> Products(string[] productExtIds)
        {
            var (response, error) = await _fireworkService.GetProductsByExtIdsAsync(productExtIds.ToList());
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null || !response.Any(product => product is not null))
                return NotFound();

            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> ShoppingCart()
        {
            var (response, error) = await _fireworkService.GetShoppingCartAsync();
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            var data = response.Select(item => new { unitId = item.Id, quantity = item.Quantity, product = item.Product }).ToList();

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> ShoppingCart(AddProductToCartModel model)
        {
            var (response, error) = await _fireworkService
                .UpdateShoppingCartItemAsync(model.ProductExtId, model.UnitExtId, model.Quantity, model.PreviousQuantity);

            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (!string.IsNullOrEmpty(response.Url))
                return Json(new { redirect = response.Url });

            var data = new { success = response.Success, message = response.Message, quantity = response.Quantity };

            return Json(data);
        }

        #endregion

        #region Instant checkout

        [HttpPost]
        public async Task<IActionResult> ShippingRates(ShippingRatesApiRequest request)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(request?.BusinessId))
            {
                try
                {
                    using var streamReader = new StreamReader(Request.Body);
                    content = await streamReader.ReadToEndAsync();
                    request = JsonConvert.DeserializeObject<ShippingRatesApiRequest>(content);
                }
                catch
                {
                    return BadRequest();
                }
            }

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return BadRequest("Not enabled");

            if (!FireworkService.IsConfigured(_fireworkSettings) || !FireworkService.IsConnected(_fireworkSettings))
                return BadRequest("Not configured");

            if (request.BusinessId != _fireworkSettings.BusinessId || request.BusinessStoreId != _fireworkSettings.BusinessStoreId)
                return BadRequest("Wrong business");

            if (!await _fireworkService.ValidateRequestAsync(Request, content))
                return BadRequest("Validation failed");

            var (response, error) = await _fireworkService.GetShippingRatesAsync(request);
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentBreakdown(PaymentBreakdownApiRequest request)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(request?.BusinessId))
            {
                try
                {
                    using var streamReader = new StreamReader(Request.Body);
                    content = await streamReader.ReadToEndAsync();
                    request = JsonConvert.DeserializeObject<PaymentBreakdownApiRequest>(content);
                }
                catch
                {
                    return BadRequest();
                }
            }

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return BadRequest("Not enabled");

            if (!FireworkService.IsConfigured(_fireworkSettings) || !FireworkService.IsConnected(_fireworkSettings))
                return BadRequest("Not configured");

            if (request.BusinessId != _fireworkSettings.BusinessId || request.BusinessStoreId != _fireworkSettings.BusinessStoreId)
                return BadRequest("Wrong business");

            if (!await _fireworkService.ValidateRequestAsync(Request, content))
                return BadRequest("Validation failed");

            var (response, error) = await _fireworkService.GetPaymentBreakdownAsync(request);
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(PlaceOrderApiRequest request)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(request?.BusinessId))
            {
                try
                {
                    using var streamReader = new StreamReader(Request.Body);
                    content = await streamReader.ReadToEndAsync();
                    request = JsonConvert.DeserializeObject<PlaceOrderApiRequest>(content);
                }
                catch
                {
                    return BadRequest();
                }
            }

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return BadRequest("Not enabled");

            if (!FireworkService.IsConfigured(_fireworkSettings) || !FireworkService.IsConnected(_fireworkSettings))
                return BadRequest("Not configured");

            if (request.BusinessId != _fireworkSettings.BusinessId || request.BusinessStoreId != _fireworkSettings.BusinessStoreId)
                return BadRequest("Wrong business");

            if (!await _fireworkService.ValidateRequestAsync(Request, content))
                return BadRequest("Validation failed");

            var (response, error) = await _fireworkService.CreateOrderAsync(request);
            if (!string.IsNullOrEmpty(error))
                return BadRequest();

            if (response is null)
                return NotFound();

            return Json(response);
        }

        #endregion

        #endregion
    }
}