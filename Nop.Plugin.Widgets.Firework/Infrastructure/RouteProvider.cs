using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.Firework.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.Firework.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: FireworkDefaults.ConfigurationRouteName,
                pattern: "Admin/Firework/Configure",
                defaults: new { controller = "Firework", action = "Configure", area = AreaNames.Admin });

            endpointRouteBuilder.MapControllerRoute(name: FireworkDefaults.OauthCallbackRouteName,
                pattern: "Admin/Firework/OauthCallback",
                defaults: new { controller = "Firework", action = "OauthCallback", area = AreaNames.Admin });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.ProductHydrate",
                pattern: "firework/v1/product-hydrate/{productExtId}",
                defaults: new { controller = "FireworkPublic", action = nameof(FireworkPublicController.ProductHydrate) });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.ShippingRates",
                pattern: "firework/v1/shipping-rates",
                defaults: new { controller = "FireworkPublic", action = nameof(FireworkPublicController.ShippingRates) });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.PaymentBreakdown",
                pattern: "firework/v1/payment-breakdown",
                defaults: new { controller = "FireworkPublic", action = nameof(FireworkPublicController.PaymentBreakdown) });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.CreateOrder",
                pattern: "firework/v1/orders",
                defaults: new { controller = "FireworkPublic", action = nameof(FireworkPublicController.CreateOrder) });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.SearchProduct",
                pattern: "firework/v1/bus/{businessId}/store/{businessStoreId}/products",
                defaults: new { controller = "FireworkOMS", action = nameof(FireworkOMSController.SearchProducts) });

            endpointRouteBuilder.MapControllerRoute(name: "Firework.GetProduct",
                pattern: "firework/v1/bus/{businessId}/store/{businessStoreId}/products/{productExtId}",
                defaults: new { controller = "FireworkOMS", action = nameof(FireworkOMSController.GetProductByExtId) });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}