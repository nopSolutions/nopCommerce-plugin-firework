using Newtonsoft.Json;
using Nop.Plugin.Widgets.Firework.Domain.Api.Products;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Order
{
    /// <summary>
    /// Represents a Firework cart item
    /// </summary>
    public class FireworkCartItem
    {
        public FireworkCartItem()
        {
            UnitOptions = new List<FireworkProductUnitOption>();
        }

        /// <summary>
        /// Gets or sets the product qunaatity
        /// </summary>
        [JsonProperty("order_quantity")]
        public int OrderQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_ext_id")]
        public string ProductExtId { get; set; }

        /// <summary>
        /// Gets or sets the product unit identifier
        /// </summary>
        [JsonProperty("unit_ext_id")]
        public string UnitExtId { get; set; }

        /// <summary>
        /// Gets or sets the product unit name
        /// </summary>
        [JsonProperty("unit_name")]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the product unit options
        /// </summary>
        [JsonProperty("unit_options")]
        public List<FireworkProductUnitOption> UnitOptions { get; set; }

        /// <summary>
        /// Gets or sets the product unit old price
        /// </summary>
        [JsonProperty("unit_original_price")]
        public decimal? UnitOriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets the product unit price
        /// </summary>
        [JsonProperty("unit_price")]
        public decimal UnitPrice { get; set; }
    }
}
