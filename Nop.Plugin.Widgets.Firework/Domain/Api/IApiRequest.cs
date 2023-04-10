namespace Nop.Plugin.Widgets.Firework.Domain.Api
{
    /// <summary>
    /// Represents request object
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the request method
        /// </summary>
        public string Method { get; }
    }
}