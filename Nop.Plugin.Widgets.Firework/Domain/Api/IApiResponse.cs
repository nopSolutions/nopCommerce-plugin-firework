namespace Nop.Plugin.Widgets.Firework.Domain.Api
{
    /// <summary>
    /// Represents response from the service
    /// </summary>
    public interface IApiResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Error { get; set; }
    }
}