using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.Firework.Domain.Api.Business;
using Nop.Plugin.Widgets.Firework.Domain.Api.OAuth;
using Nop.Plugin.Widgets.Firework.Domain.Api.Order;
using Nop.Plugin.Widgets.Firework.Domain.Api.Products;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Plugin.Widgets.Firework.Services
{
    /// <summary>
    /// Represents the plugin service
    /// </summary>
    public class FireworkService
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly FireworkHttpClient _fireworkHttpClient;
        private readonly FireworkSettings _fireworkSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FireworkService(CurrencySettings currencySettings,
            FireworkHttpClient fireworkHttpClient,
            FireworkSettings fireworkSettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            ISettingService settingService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _currencySettings = currencySettings;
            _fireworkHttpClient = fireworkHttpClient;
            _fireworkSettings = fireworkSettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _settingService = settingService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <param name="checkConfig">Whether to check configuration</param>
        /// <param name="logErrors">Whether to log errors</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; error if exists
        /// </returns>
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function,
            bool checkConfig = true, bool logErrors = true)
        {
            try
            {
                //ensure that plugin is configured
                if (checkConfig && !IsConfigured(_fireworkSettings))
                    throw new NopException("Plugin not configured");

                return (await function(), default);
            }
            catch (Exception exception)
            {
                var errorMessage = exception.Message;
                if (logErrors)
                {
                    var logMessage = $"{FireworkDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                    await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
                }

                return (default, errorMessage);
            }
        }

        /// <summary>
        /// Get content hash using HMAC secret
        /// </summary>
        /// <param name="hmacSecret">HMAC secret from FIrework</param>
        /// <param name="content">Content to hash</param>
        /// <returns>Hashed content</returns>
        private static string GetContentHash(string hmacSecret, string content)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Map a Firework product from nopCommerce product
        /// </summary>
        /// <param name="product">The product to map</param>
        /// <param name="attributesXml">Attribute XML</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the mapped product
        /// </returns>
        private async Task<FireworkProduct> MapFireworkProductFromProductAsync(Product product, string attributesXml = null)
        {
            //SKU is required
            if (string.IsNullOrEmpty(product?.Sku) || product.Deleted)
                return null;

            var store = await _storeContext.GetCurrentStoreAsync();
            var storeLocation = _webHelper.GetStoreLocation();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var seName = await _urlRecordService.GetSeNameAsync(product);
            var productUrl = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());

            var fireworkProduct = new FireworkProduct
            {
                BusinessId = _fireworkSettings.BusinessId,
                BusinessStoreId = _fireworkSettings.BusinessStoreId,
                ProductExtId = product.Sku,
                ProductName = product.Name,
                ProductDescription = !string.IsNullOrEmpty(product.ShortDescription) ? product.ShortDescription : product.Name,
                ProductCurrency = currency?.CurrencyCode ?? "USD",
                ProductHandle = productUrl,
                BusinessStoreUid = store.Url,
                BusinessStoreName = store.Name
            };

            //combination SKU is required
            var productCombinations = (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
                .Where(combination => !string.IsNullOrEmpty(combination.Sku))
                .ToList();

            if (!string.IsNullOrEmpty(attributesXml))
                productCombinations = productCombinations.Where(combination => combination.AttributesXml == attributesXml).ToList();

            if (!productCombinations.Any())
            {
                //a single unit (product details)
                fireworkProduct.ProductUnits = new List<FireworkProduct.ProductUnit> { new()
                {
                    UnitExtId = product.Sku,
                    UnitName = product.Name,
                    UnitPrice = product.Price.ToString("0.00", CultureInfo.InvariantCulture),
                    UnitOriginalPrice = product.OldPrice > decimal.Zero ? product.OldPrice.ToString("0.00", CultureInfo.InvariantCulture) : null,
                    UnitQuantity = product.StockQuantity,
                    UnitUrl = productUrl,
                    UnitDownloadable = product.IsDownload,
                    UnitPosition = 0
                } };

                //main product picture
                var picture = await _pictureService.GetProductPictureAsync(product, string.Empty);
                fireworkProduct.ProductImages = new List<FireworkProduct.ProductImage> { new()
                {
                    ImageExtId = picture?.Id.ToString(),
                    ImageAlt = picture?.AltAttribute,
                    ImageSrc = await _pictureService.GetPictureUrlAsync(picture?.Id ?? 0, storeLocation: storeLocation),
                    UnitIdentifiers = new() { product.Sku },
                    UnitNames = new() { product.Name },
                    ImagePosition = 0
                } };
            }
            else
            {
                var unitWithImages = await productCombinations.SelectAwait(async combination =>
                {
                    //unit with options (product combination details)
                    var unit = new FireworkProduct.ProductUnit
                    {
                        UnitExtId = combination.Sku,
                        UnitPrice = (combination.OverriddenPrice ?? product.Price).ToString("0.00", CultureInfo.InvariantCulture),
                        UnitOriginalPrice = null,
                        UnitQuantity = product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes
                            ? combination?.StockQuantity ?? 0
                            : product.StockQuantity / productCombinations.Count,
                        UnitUrl = productUrl,
                        UnitDownloadable = product.IsDownload,
                        UnitPosition = combination.Id
                    };

                    //unit options (product attribute name/value)
                    unit.UnitOptions = await (await _productAttributeParser.ParseProductAttributeMappingsAsync(combination.AttributesXml))
                        .SelectAwait(async mapping =>
                        {
                            var attribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
                            var values = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.AttributesXml, mapping.Id);
                            return new FireworkProductUnitOption { Name = attribute.Name, Value = values.FirstOrDefault()?.Name };
                        })
                        .ToListAsync();
                    unit.UnitName = string.Join(" / ", unit.UnitOptions.Select(option => option.Value));

                    //combination picture
                    var picture = await _pictureService.GetProductPictureAsync(product, combination.AttributesXml);
                    var image = new FireworkProduct.ProductImage
                    {
                        ImageExtId = picture?.Id.ToString(),
                        ImageAlt = picture?.AltAttribute,
                        ImageSrc = await _pictureService.GetPictureUrlAsync(picture?.Id ?? 0, storeLocation: storeLocation),
                        UnitIdentifiers = new() { unit.UnitExtId },
                        UnitNames = new() { unit.UnitName },
                        ImagePosition = unit.UnitPosition
                    };

                    return (unit, image);
                }).ToListAsync();
                fireworkProduct.ProductUnits = unitWithImages.Select(unit => unit.unit).ToList();
                fireworkProduct.ProductImages = unitWithImages.Select(image => image.image).ToList();

                //all options (product attribute names)
                var productAttributMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                var productAttributes = await productAttributMappings.SelectAwait(async mapping =>
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
                    return productAttribute.Name;
                }).ToListAsync();
                fireworkProduct.ProductOptions = productAttributes;
            }

            //additional pictures
            var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            foreach (var picture in pictures)
            {
                if (fireworkProduct.ProductImages.Any(image => image.ImageExtId == picture.Id.ToString()))
                    continue;

                fireworkProduct.ProductImages.Add(new()
                {
                    ImageExtId = picture.Id.ToString(),
                    ImageAlt = picture.AltAttribute,
                    ImageSrc = await _pictureService.GetPictureUrlAsync(picture.Id, storeLocation: storeLocation),
                    UnitIdentifiers = new() { product.Sku },
                    UnitNames = new() { product.Name },
                    ImagePosition = 1
                });
            }

            return fireworkProduct;
        }

        /// <summary>
        /// Map a Firework product from nopCommerce product
        /// </summary>
        /// <param name="productExtId">Product external identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attribute XML</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product details
        /// </returns>
        private async Task<FireworkProduct> MapFireworkProductFromProductAsync(string productExtId, Customer customer, int storeId, string attributesXml = null)
        {
            var product = (await _productService.SearchProductsAsync(keywords: productExtId, searchSku: true)).FirstOrDefault();
            var fireworkProduct = await MapFireworkProductFromProductAsync(product, attributesXml);
            if (fireworkProduct is null)
                return null;

            //update product price
            var currency = await _workContext.GetWorkingCurrencyAsync();
            fireworkProduct.ProductCurrency = currency.CurrencyCode;
            foreach (var unit in fireworkProduct.ProductUnits)
            {
                var (unitPrice, _, _) = await _shoppingCartService
                    .GetUnitPriceAsync(product, customer, ShoppingCartType.ShoppingCart, 1, attributesXml, decimal.Zero, null, null, false);
                var (finalPrice, _) = await _taxService.GetProductPriceAsync(product, unitPrice, customer);
                var price = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPrice, currency);
                unit.UnitPrice = price.ToString("0.00", CultureInfo.InvariantCulture);
            }

            return fireworkProduct;
        }

        /// <summary>
        /// Maps a nopCommerce address from Firework address
        /// </summary>
        /// <param name="fireworkAddress">Received Firework address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the mapped address, null if failed to map
        /// </returns>
        private async Task<Address> MapAddressFromFireworkAddressAsync(FireworkAddress fireworkAddress)
        {
            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(fireworkAddress.Country);
            var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country?.Id ?? 0);
            var stateProvince = states.FirstOrDefault(sp => sp.Name == fireworkAddress.State);

            return new Address
            {
                FirstName = fireworkAddress.FirstName,
                LastName = fireworkAddress.LastName,
                Email = fireworkAddress.Email,
                CountryId = country?.Id,
                StateProvinceId = stateProvince?.Id,
                City = fireworkAddress.City,
                Address1 = fireworkAddress.Line1,
                Address2 = fireworkAddress.Line2,
                ZipPostalCode = fireworkAddress.PostalCode,
                PhoneNumber = fireworkAddress.Phone
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the plugin is IsConfigured
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        public static bool IsConfigured(FireworkSettings settings)
        {
            //Client ID and Client secret are required to request services
            return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.ClientSecret);
        }

        /// <summary>
        /// Check whether the firework is connected
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        public static bool IsConnected(FireworkSettings settings)
        {
            //Business identifier & Refresh token are required to request services
            return !string.IsNullOrEmpty(settings?.BusinessId) && !string.IsNullOrEmpty(settings?.RefreshToken);
        }

        /// <summary>
        /// Validates request using HMAC secret
        /// </summary>
        /// <param name="httpRequest">Request to be validated</param>
        /// <param name="content">Request content</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true if request is valid; otherwise false
        /// </returns>
        public async Task<bool> ValidateRequestAsync(HttpRequest httpRequest, string content = null)
        {
            if (!httpRequest.Headers.TryGetValue(FireworkDefaults.HeaderTimestamp, out var timeValue) || !int.TryParse(timeValue, out var timestamp))
                return false;

            var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            if (DateTime.UtcNow - requestTime > TimeSpan.FromSeconds(900))
                return false;

            if (!httpRequest.Headers.TryGetValue(FireworkDefaults.HeaderContentSignature, out var fwContentSignature))
                return false;

            if (string.IsNullOrEmpty(content))
            {
                content = httpRequest.Method == HttpMethods.Get ? UriHelper.GetDisplayUrl(httpRequest) : string.Empty;
                if (httpRequest.Method == HttpMethods.Post)
                {
                    using var streamReader = new StreamReader(httpRequest.Body);
                    content = await streamReader.ReadToEndAsync();
                }
            }
            if (string.IsNullOrEmpty(content))
                return false;

            try
            {
                var (hmacSecret, _) = await GetHmacSecretAsync();
                var contentHash = GetContentHash(hmacSecret ?? string.Empty, content);
                return string.Equals(contentHash, fwContentSignature, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        #region OAuth

        /// <summary>
        /// Get OAuth callback URL
        /// </summary>
        /// <returns>URL</returns>
        public string GetOAuthCallBackUrl()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.RouteUrl(FireworkDefaults.OauthCallbackRouteName, null, _webHelper.GetCurrentRequestProtocol()).ToLowerInvariant();
        }

        /// <summary>
        /// Register OAuth application
        /// </summary>
        /// <param name="email">Merchant email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the OAuth client details; error message if exists
        /// </returns>
        public async Task<(OAuthClientResponse Result, string Error)> RegisterOAuthAsync(string email)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            return await HandleFunctionAsync(async () => await _fireworkHttpClient.RequestAsync<RegisterRequest, OAuthClientResponse>(new()
            {
                ClientName = FireworkDefaults.ApplicationName,
                RedirectUris = new string[] { GetOAuthCallBackUrl() },
                Contacts = new string[] { email, customer.Email },
                Scope = "openid"
            }), checkConfig: false);
        }

        /// <summary>
        /// Get authorization URL
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the URL to authorize
        /// </returns>
        public async Task<string> GetAuthorizationUrlAsync()
        {
            var url = $"{(_fireworkSettings.UseSandbox ? FireworkDefaults.SandboxApiUrl : FireworkDefaults.ApiUrl)}oauth/authorize";

            var parameters = new Dictionary<string, string>
            {
                ["client"] = "business",
                ["response_type"] = "code",
                ["client_id"] = _fireworkSettings.ClientId,
                ["redirect_uri"] = GetOAuthCallBackUrl(),
            };

            //onboarding
            if (string.IsNullOrEmpty(_fireworkSettings.BusinessId))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                parameters.Add("business_onboard", "true");
                parameters.Add("business", store.Name);
                parameters.Add("website", store.Url);
                parameters.Add("email", _fireworkSettings.Email);
            }

            return QueryHelpers.AddQueryString(url, parameters);
        }

        /// <summary>
        /// Handle OAuth callback request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the check result; error message if exists
        /// </returns>
        public async Task<(bool result, string Error)> HandleOauthCallbackAsync(HttpRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!request.Query.TryGetValue("code", out var authCode))
                    throw new NopException("Authorization code not found");

                var authentication = await _fireworkHttpClient.RequestAsync<OAuthApiRequest, OAuthResponse>(new()
                {
                    GrantType = "authorization_code",
                    RedirectUri = GetOAuthCallBackUrl(),
                    ClientId = _fireworkSettings.ClientId,
                    ClientSecret = _fireworkSettings.ClientSecret,
                    AuthCode = authCode
                });

                if (request.Query.TryGetValue("business_id", out var businessId))
                    _fireworkSettings.BusinessId = businessId;

                _fireworkSettings.RefreshToken = authentication.RefreshToken;
                _fireworkSettings.AccessToken = authentication.AccessToken;
                _fireworkSettings.RefreshTokenExpiresIn = DateTime.Now.AddSeconds(authentication.RefreshTokenExpiresIn);
                _fireworkSettings.TokenExpiresIn = DateTime.Now.AddSeconds(authentication.TokenExpiresIn);

                await _settingService.SaveSettingAsync(_fireworkSettings);

                return true;
            });
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the access token; error message if exists
        /// </returns>
        public async Task<(string Token, string Error)> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_fireworkSettings.AccessToken) && _fireworkSettings.TokenExpiresIn >= DateTime.Now)
                return (_fireworkSettings.AccessToken, string.Empty);

            return await HandleFunctionAsync(async () =>
            {
                var result = await _fireworkHttpClient.RequestAsync<OAuthApiRequest, OAuthResponse>(new()
                {
                    GrantType = "refresh_token",
                    RedirectUri = GetOAuthCallBackUrl(),
                    ClientId = _fireworkSettings.ClientId,
                    ClientSecret = _fireworkSettings.ClientSecret,
                    RefreshToken = _fireworkSettings.RefreshToken,
                });

                _fireworkSettings.AccessToken = result.AccessToken;
                _fireworkSettings.TokenExpiresIn = DateTime.Now.AddSeconds(result.TokenExpiresIn);
                _fireworkSettings.RefreshToken = result.RefreshToken;
                _fireworkSettings.RefreshTokenExpiresIn = DateTime.Now.AddSeconds(result.RefreshTokenExpiresIn);
                await _settingService.SaveSettingAsync(_fireworkSettings);

                return result.AccessToken;
            });
        }

        #endregion

        #region Products

        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains update result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> UpdateProductAsync(Product product)
        {
            return await HandleFunctionAsync(async () =>
            {
                var fireworkProduct = await MapFireworkProductFromProductAsync(product);
                if (fireworkProduct is null)
                    return false;

                var request = UpdateProductRequest.FromFireworkProduct(fireworkProduct);
                (request.Token, _) = await GetAccessTokenAsync();

                var content = JsonConvert.SerializeObject(request);
                var (hmacSecret, _) = await GetHmacSecretAsync();
                var contentHash = GetContentHash(hmacSecret ?? string.Empty, content);
                request.ContentSignature = contentHash;

                var response = await _fireworkHttpClient.RequestAsync<UpdateProductRequest, UpdateProductResponse>(request);
                var result = string.Equals(response?.Result, "successful", StringComparison.InvariantCultureIgnoreCase);

                return result;
            }, logErrors: false);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="productId">Product id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains delete result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> DeleteProductAsync(int productId)
        {
            return await HandleFunctionAsync(async () =>
            {
                var request = new DeleteProductRequest
                {
                    ProductExtId = productId.ToString()
                };
                (request.Token, _) = await GetAccessTokenAsync();

                var content = JsonConvert.SerializeObject(request);
                var (hmacSecret, _) = await GetHmacSecretAsync();
                var contentHash = GetContentHash(hmacSecret ?? string.Empty, content);
                request.ContentSignature = contentHash;

                var response = await _fireworkHttpClient.RequestAsync<DeleteProductRequest, DeleteProductResponse>(request);
                var result = string.Equals(response?.Result, "successful", StringComparison.InvariantCultureIgnoreCase);

                return result;
            }, logErrors: false);
        }

        #endregion

        #region Business

        /// <summary>
        /// Get HMAC secret from Firework
        /// </summary>
        /// <param name="fireworkSettings">Settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains HMAC secret; error message if exists
        /// </returns>
        public async Task<(string Result, string Error)> GetHmacSecretAsync(FireworkSettings fireworkSettings = null)
        {
            fireworkSettings ??= _fireworkSettings;

            if (!string.IsNullOrEmpty(_fireworkSettings.HmacSecret))
                return (_fireworkSettings.HmacSecret, null);

            //try to get new one
            return await HandleFunctionAsync(async () =>
            {
                var request = new HmacApiRequest
                {
                    BusinessStoreId = _fireworkSettings.BusinessStoreId
                };
                (request.Token, _) = await GetAccessTokenAsync();

                var response = await _fireworkHttpClient.RequestAsync<HmacApiRequest, HmacApiResponse>(request);
                var result = response?.Data?.BusinessStoreShuffleHmacSecret?.HmacSecret;

                _fireworkSettings.HmacSecret = result;
                await _settingService.SaveSettingAsync(_fireworkSettings);

                return result;
            });
        }

        /// <summary>
        /// Get all business stores
        /// </summary>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of all stores and the selected store; error if exists
        /// </returns>
        public async Task<((IList<BusinessStore> Stores, string SelectedStoreId), string Error)> GetBusinessStoresAsync(int? pageSize = null)
        {
            //get all stores
            var (existingStores, error) = await HandleFunctionAsync(async () =>
            {
                var request = new GetBusinessStoresRequest()
                {
                    BusinessId = _fireworkSettings.BusinessId,
                    PageSize = pageSize ?? 0,
                };
                (request.Token, _) = await GetAccessTokenAsync();
                var result = await _fireworkHttpClient.RequestAsync<GetBusinessStoresRequest, GetBusinessStoresResponse>(request);
                return result.BusinessStores ?? new List<BusinessStore>();
            });

            if (!string.IsNullOrEmpty(_fireworkSettings.BusinessStoreId))
            {
                //whether the selected store is exists
                var businessStoreId = existingStores.Any(businessStore => businessStore.Id == _fireworkSettings.BusinessStoreId)
                    ? _fireworkSettings.BusinessStoreId
                    : string.Empty;
                return ((existingStores, businessStoreId), error);
            }

            //or try create a new store
            var store = await _storeContext.GetCurrentStoreAsync();
            var currentStore = existingStores
                .FirstOrDefault(businessStore => businessStore.Uid?.Equals(store.Url, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (currentStore is not null)
                return ((existingStores, currentStore.Id), error);

            var (newBusinessStore, storeError) = await HandleFunctionAsync(async () =>
            {
                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                var request = new CreateBusinessStoreRequest
                {
                    BusinessId = _fireworkSettings.BusinessId,
                    Currency = currency?.CurrencyCode ?? "USD",
                    Name = store.Name,
                    Uid = store.Url,
                    Url = store.Url,
                    ApiUrl = store.Url
                };
                (request.Token, _) = await GetAccessTokenAsync();
                var response = await _fireworkHttpClient.RequestAsync<CreateBusinessStoreRequest, CreateBusinessStoreResponse>(request);
                return response?.Data?.Result;
            });

            if (newBusinessStore is not null)
                existingStores.Add(new BusinessStore { Id = newBusinessStore.Id, Name = newBusinessStore.Name });

            return ((existingStores, newBusinessStore?.Id ?? string.Empty), error);
        }

        /// <summary>
        /// Get all channels of a store
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of channels; error if exists
        /// </returns>
        public async Task<(List<Channel> channels, string error)> GetChannelsAsync()
        {
            if (string.IsNullOrEmpty(_fireworkSettings.BusinessId))
                return (new(), string.Empty);

            var (result, error) = await HandleFunctionAsync(async () =>
            {
                var request = new GetChannelsRequest
                {
                    BusinessId = _fireworkSettings.BusinessId,
                };
                (request.Token, _) = await GetAccessTokenAsync();
                return await _fireworkHttpClient.RequestAsync<GetChannelsRequest, GetChannelsResponse>(request);
            });

            return (result.Channels ?? new(), error);
        }

        /// <summary>
        /// Get playlists by channel identifier
        /// </summary>
        /// <param name="channelId">Channel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of playlists; error if exists
        /// </returns>
        public async Task<(List<Playlist> playlists, string error)> GetPlaylistsAsync(string channelId)
        {
            if (!IsConnected(_fireworkSettings))
                return (new(), string.Empty);

            if (string.IsNullOrEmpty(channelId))
                return (new(), string.Empty);

            var (result, error) = await HandleFunctionAsync(async () =>
            {
                var request = new GetPlaylistsRequest
                {
                    BusinessId = _fireworkSettings.BusinessId,
                    ChannelId = channelId
                };
                (request.Token, _) = await GetAccessTokenAsync();

                return await _fireworkHttpClient.RequestAsync<GetPlaylistsRequest, GetPlaylistsResponse>(request);
            });

            return (result.Playlists ?? new(), error);
        }

        /// <summary>
        /// Get videos
        /// </summary>
        /// <param name="channelId">Channel identifier</param>
        /// <param name="playlistId">Playlist identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of videos; error if exists
        /// </returns>
        public async Task<(List<Video> Videos, string Error)> GetVideosAsync(string channelId, string playlistId)
        {
            if (!IsConnected(_fireworkSettings))
                return (new(), string.Empty);

            if (string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(playlistId))
                return (new(), string.Empty);

            var (result, error) = await HandleFunctionAsync(async () =>
            {
                var request = new GetVideosRequest
                {
                    BusinessId = _fireworkSettings.BusinessId,
                    ChannelId = channelId,
                    PlaylistId = playlistId
                };
                (request.Token, _) = await GetAccessTokenAsync();

                return await _fireworkHttpClient.RequestAsync<GetVideosRequest, GetVideosResponse>(request);
            });

            return (result.Videos ?? new(), error);
        }

        #endregion

        #region OMS API

        /// <summary>
        /// Get products to import
        /// </summary>
        /// <param name="search">Keywords to search</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of products; error if exists
        /// </returns>
        public async Task<(SearchProductsApiResponse Result, string Error)> GetImportProductsAsync(string search, int page, int pageSize)
        {
            return await HandleFunctionAsync(async () =>
            {
                var products = await _productService.SearchProductsAsync(keywords: search, searchSku: true, pageIndex: page - 1, pageSize: pageSize);
                return new SearchProductsApiResponse
                {
                    TotalEntries = products.TotalCount,
                    PageNumber = products.PageIndex + 1,
                    TotalPages = products.TotalPages,
                    PageSize = products.PageSize,
                    Entries = await products
                        .SelectAwait(async product => await MapFireworkProductFromProductAsync(product))
                        .Where(product => product is not null)
                        .ToListAsync()
                };
            });
        }

        /// <summary>
        /// Get single product to import
        /// </summary>
        /// <param name="productExtId">Product id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product; error if exists
        /// </returns>
        public async Task<(FireworkProduct Product, string Error)> GetImportProductAsync(string productExtId)
        {
            return await HandleFunctionAsync(async () =>
            {
                var product = (await _productService.SearchProductsAsync(keywords: productExtId, searchSku: true)).FirstOrDefault();
                if (product is null)
                    return null;

                return await MapFireworkProductFromProductAsync(product);
            });
        }

        #endregion

        #region Native checkout

        /// <summary>
        /// Get products by external identifiers
        /// </summary>
        /// <param name="productExtIds">Product ids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of products; error if exists
        /// </returns>
        public async Task<(List<FireworkProduct> Products, string Error)> GetProductsByExtIdsAsync(List<string> productExtIds)
        {
            return await HandleFunctionAsync(async () =>
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var products = await productExtIds
                    .SelectAwait(async productExtId => await MapFireworkProductFromProductAsync(productExtId, customer, store.Id))
                    .Where(product => product is not null)
                    .ToListAsync();

                return products;
            });
        }

        /// <summary>
        /// Get current shopping cart
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of shopping cart items; error if exists
        /// </returns>
        public async Task<(List<(string Id, int Quantity, FireworkProduct Product)> Items, string Error)> GetShoppingCartAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();

                var items = new List<(string Id, int Quantity, FireworkProduct Product)>();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                foreach (var item in cart)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var productExtId = await _productService.FormatSkuAsync(product, item.AttributesXml);
                    var fireworkProduct = await MapFireworkProductFromProductAsync(productExtId, customer, store.Id, item.AttributesXml);
                    if (fireworkProduct is not null)
                        items.Add((productExtId, item.Quantity, fireworkProduct));
                }

                return items;
            });
        }

        /// <summary>
        /// Update shopping cart item
        /// </summary>
        /// <param name="productExtId">Product id</param>
        /// <param name="unitExtId">Unit id</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="previousQuantity">Previous quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the updated shopping cart item; error if exists
        /// </returns>
        public async Task<((string Url, bool Success, string Message, int Quantity) Result, string Error)> UpdateShoppingCartItemAsync(string productExtId,
            string unitExtId, int quantity, int previousQuantity)
        {
            return await HandleFunctionAsync(async () =>
            {
                var product = await _productService.GetProductBySkuAsync(productExtId);
                if (product is null)
                    return (null, false, "No product found with the specified ID", 0);

                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var redirectUrl = urlHelper.RouteUrl("Product", new { SeName = await _urlRecordService.GetSeNameAsync(product) });

                //we can add only simple products
                if (product.ProductType != ProductType.SimpleProduct)
                    return (redirectUrl, false, string.Empty, 0);

                //cannot be added to the cart (requires a customer to enter price)
                if (product.CustomerEntersPrice)
                    return (redirectUrl, false, string.Empty, 0);

                //rental products require start/end dates to be entered
                if (product.IsRental)
                    return (redirectUrl, false, string.Empty, 0);

                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                var allowedQuantities = _productService.ParseAllowedQuantities(product);
                if (allowedQuantities.Length > 0)
                    return (redirectUrl, false, string.Empty, 0);

                //get attribute combination by SKU
                var attributes = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(unitExtId);

                //get standard warnings without attribute validations
                //first, try to find existing shopping cart item
                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                var shoppingCartItem = await _shoppingCartService
                    .FindShoppingCartItemInTheCartAsync(cart, ShoppingCartType.ShoppingCart, product, attributes?.AttributesXml);

                var delete = false;
                if (quantity <= 0 && shoppingCartItem is not null)
                {
                    await _shoppingCartService.DeleteShoppingCartItemAsync(shoppingCartItem.Id);
                    delete = true;
                }

                if (!delete)
                {
                    //if we already have the same product in the cart, then use the total quantity to validate
                    var quantityToValidate = quantity;
                    var addToCartWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(customer, ShoppingCartType.ShoppingCart,
                        product, store.Id, attributes?.AttributesXml,
                        decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, true, false, false);
                    if (addToCartWarnings.Any())
                    {
                        //cannot be added to the cart
                        //let's display standard warnings
                        return (null, false, string.Join(';', addToCartWarnings), 0);
                    }

                    if (quantity > (shoppingCartItem?.Quantity ?? 0))
                    {
                        //now let's try adding product to the cart (now including product attribute validation, etc)
                        addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                            product: product,
                            shoppingCartType: ShoppingCartType.ShoppingCart,
                            storeId: store.Id,
                            attributesXml: attributes?.AttributesXml,
                            quantity: quantity - (shoppingCartItem?.Quantity ?? 0));
                    }
                    else if (shoppingCartItem is not null)
                    {
                        var updateWarnings = await _shoppingCartService
                            .UpdateShoppingCartItemAsync(customer, shoppingCartItem.Id, attributes?.AttributesXml, 0, null, null, quantity);
                        addToCartWarnings = addToCartWarnings.Concat(updateWarnings).ToList();
                    }

                    if (addToCartWarnings.Any())
                    {
                        //cannot be added to the cart
                        return (redirectUrl, false, string.Empty, 0);
                    }
                }

                //display notification message and update appropriate blocks
                var updatedCart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                var updatedItem = await _shoppingCartService
                    .FindShoppingCartItemInTheCartAsync(updatedCart, ShoppingCartType.ShoppingCart, product, attributes?.AttributesXml);

                var message = delete
                    ? await _localizationService.GetResourceAsync("Plugins.Widgets.Firework.ShoppingCart.ItemRemoved")
                    : string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"), urlHelper.RouteUrl("ShoppingCart"));

                return (null, true, message, updatedItem?.Quantity ?? 0);
            });
        }

        #endregion

        #region Instant checkout

        /// <summary>
        /// Get shipping rates
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of shipping options; error if exists
        /// </returns>
        public async Task<(ShippingRatesApiResponse Result, string Error)> GetShippingRatesAsync(ShippingRatesApiRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                var shippingAddress = await MapAddressFromFireworkAddressAsync(request.ShippingAddress)
                    ?? throw new NopException("Failed to get shipping address");

                var customer = (request.Metadata?.CustomerGuid is not null && Guid.TryParse(request.Metadata.CustomerGuid, out var customerGuid)
                    ? await _customerService.GetCustomerByGuidAsync(customerGuid)
                    //: await _customerService.GetCustomerByEmailAsync(request.Metadata?.Email);
                    : null)
                    ?? throw new NopException("Customer not found");

                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                foreach (var sci in cart)
                {
                    await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
                }

                foreach (var item in request.CartItems)
                {
                    var product = (await _productService.SearchProductsAsync(keywords: item.ProductExtId, searchSku: true)).FirstOrDefault();
                    if (product is null || product.Deleted)
                        continue;

                    var attributes = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(item.UnitExtId);
                    await _shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, store.Id, attributes?.AttributesXml,
                        quantity: item.OrderQuantity, addRequiredProducts: false);
                }

                cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                var shippingRates = await _shippingService.GetShippingOptionsAsync(cart, shippingAddress, customer, storeId: store.Id);
                var (_, _, subtotal, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                var (total, discount, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, usePaymentMethodAdditionalFee: false);

                var response = new ShippingRatesApiResponse
                {
                    Currency = currency?.CurrencyCode ?? "USD",
                    Metadata = new FireworkCartMetadata
                    {
                        CustomerGuid = customer.CustomerGuid.ToString(),
                        Email = customer.Email
                    },
                    Subtotal = subtotal.ToString("0.00", CultureInfo.InvariantCulture),
                    Total = (total ?? 0).ToString("0.00", CultureInfo.InvariantCulture),
                    Discount = discount.ToString("0.00", CultureInfo.InvariantCulture),
                    Tax = null,
                    Shipping = null,
                    ShippingRates = shippingRates.ShippingOptions.Select(so => new FireworkShippingRate
                    {
                        Id = $"{so.ShippingRateComputationMethodSystemName};{so.Rate}",
                        Title = so.Name,
                        Cost = so.Rate.ToString("0.00", CultureInfo.InvariantCulture)
                    }).ToList(),
                    CartItems = request.CartItems
                };

                return response;
            });
        }

        /// <summary>
        /// Get payment breakdown
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment details; error if exists
        /// </returns>
        public async Task<(PaymentBreakdownApiResponse Result, string Error)> GetPaymentBreakdownAsync(PaymentBreakdownApiRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (request.Metadata?.CustomerGuid is null || !Guid.TryParse(request.Metadata.CustomerGuid, out var customerGuid))
                    throw new NopException("Customer not found");

                var customer = await _customerService.GetCustomerByGuidAsync(customerGuid)
                    ?? throw new NopException("Customer not found");

                var billingAddress = await MapAddressFromFireworkAddressAsync(request.BillingAddress)
                    ?? throw new NopException("Failed to get billing address");

                var shippingAddress = await MapAddressFromFireworkAddressAsync(request.ShippingAddress)
                    ?? throw new NopException("Failed to get shipping address");

                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                foreach (var sci in cart)
                {
                    await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
                }

                foreach (var item in request.CartItems)
                {
                    var product = (await _productService.SearchProductsAsync(keywords: item.ProductExtId, searchSku: true)).FirstOrDefault();
                    if (product is null || product.Deleted)
                        continue;

                    var attributes = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(item.UnitExtId);
                    await _shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, store.Id, attributes?.AttributesXml,
                        quantity: item.OrderQuantity, addRequiredProducts: false);
                }

                //set billing address
                var customerAddresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
                var address = _addressService.FindAddress(customerAddresses.ToList(),
                    billingAddress.FirstName, billingAddress.LastName, billingAddress.PhoneNumber,
                    billingAddress.Email, billingAddress.FaxNumber, billingAddress.Company,
                    billingAddress.Address1, billingAddress.Address2, billingAddress.City,
                    billingAddress.County, billingAddress.StateProvinceId, billingAddress.ZipPostalCode,
                    billingAddress.CountryId, null);
                if (address is null)
                {
                    address = billingAddress;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    await _addressService.InsertAddressAsync(address);
                    await _customerService.InsertCustomerAddressAsync(customer, address);
                }
                customer.BillingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(customer);

                //set shipping address
                address = _addressService.FindAddress(customerAddresses.ToList(),
                    shippingAddress.FirstName, shippingAddress.LastName, shippingAddress.PhoneNumber,
                    shippingAddress.Email, shippingAddress.FaxNumber, shippingAddress.Company,
                    shippingAddress.Address1, shippingAddress.Address2, shippingAddress.City,
                    shippingAddress.County, shippingAddress.StateProvinceId, shippingAddress.ZipPostalCode,
                    shippingAddress.CountryId, null);
                if (address is null)
                {
                    address = shippingAddress;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    await _addressService.InsertAddressAsync(address);
                    await _customerService.InsertCustomerAddressAsync(customer, address);
                }
                customer.ShippingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(customer);

                //set shipping option
                cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                var shippingRates = await _shippingService.GetShippingOptionsAsync(cart, shippingAddress, customer, storeId: store.Id);

                var optionName = request.ShippingId?.Split(';', StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault();
                var optionRateValue = request.ShippingId?.Split(';', StringSplitOptions.RemoveEmptyEntries)?.LastOrDefault();
                _ = decimal.TryParse(optionRateValue, out var optionRate);
                var shippingOption = shippingRates.ShippingOptions
                    ?.FirstOrDefault(option => option.Rate == optionRate && option.ShippingRateComputationMethodSystemName == optionName);
                await _genericAttributeService
                    .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

                //set payment method
                await _genericAttributeService
                    .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, FireworkDefaults.SystemName, store.Id);

                //calculate totals
                var (_, _, subTotal, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                var (shipping, _, _) = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, false);
                var (taxTotal, _) = await _orderTotalCalculationService.GetTaxTotalAsync(cart, usePaymentMethodAdditionalFee: false);
                var (total, discount, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, usePaymentMethodAdditionalFee: false);

                //place order
                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(new ProcessPaymentRequest
                {
                    StoreId = store.Id,
                    CustomerId = customer.Id,
                    PaymentMethodSystemName = FireworkDefaults.SystemName,
                    OrderGuid = Guid.NewGuid(),
                    OrderGuidGeneratedOnUtc = DateTime.Now,
                    OrderTotal = total ?? 0,
                });
                if (!placeOrderResult.Success)
                    throw new NopException("Failed to get order totals");

                var response = new PaymentBreakdownApiResponse
                {
                    Currency = currency?.CurrencyCode ?? "USD",
                    Metadata = new FireworkCartMetadata
                    {
                        CustomerGuid = customer.CustomerGuid.ToString(),
                        Email = customer.Email,
                        OrderGuid = placeOrderResult.PlacedOrder.OrderGuid.ToString()
                    },
                    Discount = discount.ToString("0.00", CultureInfo.InvariantCulture),
                    Subtotal = subTotal.ToString("0.00", CultureInfo.InvariantCulture),
                    Shipping = (shipping ?? 0).ToString("0.00", CultureInfo.InvariantCulture),
                    Tax = taxTotal.ToString("0.00", CultureInfo.InvariantCulture),
                    Total = (total ?? 0).ToString("0.00", CultureInfo.InvariantCulture),
                    CartItems = request.CartItems
                };

                return response;
            });
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order details; error if exists
        /// </returns>
        public async Task<(PlaceOrderApiResponse Result, string Error)> CreateOrderAsync(PlaceOrderApiRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (request.Payment?.PaymentDetails is not PaymentDetails payment)
                    throw new NopException("Payment not found");

                if (request.Metadata?.CustomerGuid is null || !Guid.TryParse(request.Metadata.CustomerGuid, out var customerGuid))
                    throw new NopException("Customer not found");

                var customer = await _customerService.GetCustomerByGuidAsync(customerGuid)
                    ?? throw new NopException("Customer not found");

                if (request.Metadata?.OrderGuid is null || !Guid.TryParse(request.Metadata.OrderGuid, out var orderGuid))
                    throw new NopException("Order not found");

                var order = await _orderService.GetOrderByGuidAsync(orderGuid)
                    ?? throw new NopException("Order not found");

                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    await _orderService.InsertOrderNoteAsync(new()
                    {
                        OrderId = order.Id,
                        Note = $"Payment details: Provider - {payment.PaymentProvider}, " +
                            $"Reference - {payment.PaymentReference}, Firework order id - {payment.FireworkOrderId}, URL - {payment.PaymentDetailUrl}",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    order.CaptureTransactionId = payment.PaymentReference;
                    await _orderService.UpdateOrderAsync(order);
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);
                }

                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                var response = new PlaceOrderApiResponse
                {
                    Currency = currency?.CurrencyCode ?? "USD",
                    Metadata = new FireworkCartMetadata
                    {
                        CustomerGuid = customer.CustomerGuid.ToString(),
                        Email = customer.Email,
                        OrderGuid = order.OrderGuid.ToString()
                    },
                    BusinessStoreId = _fireworkSettings.BusinessStoreId,
                    ExternalOrderId = order.CustomOrderNumber,
                    ReceiptEmail = request.ReceiptEmail,
                    Shipping = order.OrderShippingExclTax.ToString("0.00", CultureInfo.InvariantCulture),
                    Subtotal = order.OrderSubtotalExclTax.ToString("0.00", CultureInfo.InvariantCulture),
                    Discount = (order.OrderDiscount + order.OrderSubTotalDiscountExclTax).ToString("0.00", CultureInfo.InvariantCulture),
                    Tax = order.OrderTax.ToString("0.00", CultureInfo.InvariantCulture),
                    Total = order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture),
                    Payment = request.Payment,
                    CartItems = request.CartItems,
                };

                return response;
            });
        }

        #endregion

        #endregion
    }
}