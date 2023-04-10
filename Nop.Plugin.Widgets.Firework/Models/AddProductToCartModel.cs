using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Firework.Models
{
    public record AddProductToCartModel : BaseNopModel
    {
        public string ProductExtId { get; set; }

        public string UnitExtId { get; set; }

        public int Quantity { get; set; }

        public int PreviousQuantity { get; set; }
    }
}