using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.Firework.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductAttribute>>,
        IConsumer<EntityDeletedEvent<ProductAttribute>>,
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
        IConsumer<EntityDeletedEvent<ProductAttributeValue>>,
        IConsumer<EntityInsertedEvent<ProductAttributeCombination>>,
        IConsumer<EntityUpdatedEvent<ProductAttributeCombination>>,
        IConsumer<EntityDeletedEvent<ProductAttributeCombination>>

    {
        #region Fields

        private readonly FireworkService _fireworkService;
        private readonly FireworkSettings _fireworkSettings;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IWidgetPluginManager _widgetPluginManager;

        #endregion

        #region Ctor

        public EventConsumer(FireworkService fireworkService,
            FireworkSettings fireworkSettings,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IWidgetPluginManager widgetPluginManager)
        {
            _fireworkService = fireworkService;
            _fireworkSettings = fireworkSettings;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _widgetPluginManager = widgetPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            if (!eventMessage.Entity.Deleted)
                await _fireworkService.UpdateProductAsync(eventMessage.Entity);
            else
                await _fireworkService.DeleteProductAsync(eventMessage.Entity.Id);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            await _fireworkService.DeleteProductAsync(eventMessage.Entity.Id);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var products = await _productService.GetProductsByProductAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var product in products)
            {
                await _fireworkService.UpdateProductAsync(product);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var products = await _productService.GetProductsByProductAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var product in products)
            {
                await _fireworkService.UpdateProductAsync(product);
            }
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var mapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(eventMessage.Entity.ProductAttributeMappingId);
            if (mapping is null)
                return;

            var product = await _productService.GetProductByIdAsync(mapping.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var mapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(eventMessage.Entity.ProductAttributeMappingId);
            if (mapping is null)
                return;

            var product = await _productService.GetProductByIdAsync(mapping.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!await _widgetPluginManager.IsPluginActiveAsync(FireworkDefaults.SystemName))
                return;

            if (!FireworkService.IsConfigured(_fireworkSettings))
                return;

            var product = await _productService.GetProductByIdAsync(eventMessage.Entity.ProductId);
            await _fireworkService.UpdateProductAsync(product);
        }

        #endregion
    }
}