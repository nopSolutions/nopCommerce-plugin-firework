using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Products
{
    /// <summary>
    /// Represents a search products response
    /// </summary>
    public class SearchProductsApiResponse : ProductListApiResponse
    {
    }

    /// <summary>
    /// Represents a product list response
    /// </summary>
    public class ProductListApiResponse : PagedResponse<FireworkProduct>
    {
        public ProductListApiResponse()
        {
            Entries = new List<FireworkProduct>();
        }
    }

    /// <summary>
    /// Represents a product response
    /// </summary>
    public class ProductApiResponse : FireworkProduct, IApiResponse
    {
        public string Error { get; set; }
    }
}