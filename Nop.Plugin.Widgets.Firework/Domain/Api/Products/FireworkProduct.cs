using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Firework.Domain.Api.Products
{
    /// <summary>
    /// Represents a Firework product
    /// </summary>
    public class FireworkProduct
    {
        public FireworkProduct()
        {
            ProductUnits = new List<ProductUnit>();
            ProductOptions = new List<string>();
            ProductImages = new List<ProductImage>();
        }

        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        [JsonProperty("business_id")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the business store identifier
        /// </summary>
        [JsonProperty("business_store_id")]
        public string BusinessStoreId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_ext_id")]
        public string ProductExtId { get; set; }

        /// <summary>
        /// Gets or sets the product currency
        /// </summary>
        [JsonProperty("product_currency")]
        public string ProductCurrency { get; set; }

        /// <summary>
        /// Gets or sets the product URL
        /// </summary>
        [JsonProperty("product_handle")]
        public string ProductHandle { get; set; }

        /// <summary>
        /// Gets or sets the business store name
        /// </summary>
        [JsonProperty("business_store_name")]
        public string BusinessStoreName { get; set; }

        /// <summary>
        /// Gets or sets the business store UID
        /// </summary>
        [JsonProperty("business_store_uid")]
        public string BusinessStoreUid { get; set; }

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the product description
        /// </summary>
        [JsonProperty("product_description")]
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the product options
        /// </summary>
        [JsonProperty("product_options")]
        public List<string> ProductOptions { get; set; }

        /// <summary>
        /// Gets or sets the product images
        /// </summary>
        [JsonProperty("product_images")]
        public List<ProductImage> ProductImages { get; set; }

        /// <summary>
        /// Gets or sets the product units
        /// </summary>
        [JsonProperty("product_units")]
        public List<ProductUnit> ProductUnits { get; set; }

        #region Nested classes

        /// <summary>
        /// Represents a Firework product image
        /// </summary>
        public class ProductImage
        {
            public ProductImage()
            {
                UnitIdentifiers = new List<string>();
                UnitNames = new List<string>();
            }

            /// <summary>
            /// Gets or sets the image identifier
            /// </summary>
            [JsonProperty("image_ext_id")]
            public string ImageExtId { get; set; }

            /// <summary>
            /// Gets or sets the image display order
            /// </summary>
            [JsonProperty("image_position")]
            public int ImagePosition { get; set; }

            /// <summary>
            /// Gets or sets the image URL
            /// </summary>
            [JsonProperty("image_src")]
            public string ImageSrc { get; set; }

            /// <summary>
            /// Gets or sets the image alt
            /// </summary>
            [JsonProperty("image_alt")]
            public string ImageAlt { get; set; }

            /// <summary>
            /// Gets or sets the image unit identifiers
            /// </summary>
            [JsonProperty("unit_identifiers")]
            public List<string> UnitIdentifiers { get; set; }

            /// <summary>
            /// Gets or sets the image unit names
            /// </summary>
            [JsonProperty("unit_names")]
            public List<string> UnitNames { get; set; }
        }

        /// <summary>
        /// Represents a Firework product unit
        /// </summary>
        public class ProductUnit
        {
            public ProductUnit()
            {
                UnitOptions = new List<FireworkProductUnitOption>();
            }

            /// <summary>
            /// Gets or sets a value indicating whether the unit is downloadable
            /// </summary>
            [JsonProperty("unit_downloadable")]
            public bool UnitDownloadable { get; set; }

            /// <summary>
            /// Gets or sets the unit identifier
            /// </summary>
            [JsonProperty("unit_ext_id")]
            public string UnitExtId { get; set; }

            /// <summary>
            /// Gets or sets the unit name
            /// </summary>
            [JsonProperty("unit_name")]
            public string UnitName { get; set; }

            /// <summary>
            /// Gets or sets the unit options
            /// </summary>
            [JsonProperty("unit_options")]
            public List<FireworkProductUnitOption> UnitOptions { get; set; }

            /// <summary>
            /// Gets or sets the unit position
            /// </summary>
            [JsonProperty("unit_position")]
            public int UnitPosition { get; set; }

            /// <summary>
            /// Gets or sets the unit old price
            /// </summary>
            [JsonProperty("unit_original_price")]
            public string UnitOriginalPrice { get; set; }

            /// <summary>
            /// Gets or sets the unit price
            /// </summary>
            [JsonProperty("unit_price")]
            public string UnitPrice { get; set; }

            /// <summary>
            /// Gets or sets the unit quantity
            /// </summary>
            [JsonProperty("unit_quantity")]
            public int UnitQuantity { get; set; }

            /// <summary>
            /// Gets or sets the unit URL
            /// </summary>
            [JsonProperty("unit_url")]
            public string UnitUrl { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// Represents a Firework product unit option
    /// </summary>
    public class FireworkProductUnitOption
    {
        /// <summary>
        /// Gets or sets the option name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the option value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}