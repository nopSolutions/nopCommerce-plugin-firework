namespace Nop.Plugin.Widgets.Firework.Domain.Api
{
    /// <summary>
    /// Represents request with authorization
    /// </summary>
    public interface IAuthorizedRequest
    {
        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public string Token { get; set; }
    }
}