using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Firework.Models
{
    /// <summary>
    /// Represents a portal credentials model
    /// </summary>
    public record PortalModel : BaseNopModel
    {
        #region Properties

        public string BusinessId { get; set; }

        public string BusinessStoreId { get; set; }

        public string AccessToken { get; set; }

        #endregion
    }
}