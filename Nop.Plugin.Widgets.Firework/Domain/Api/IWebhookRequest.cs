namespace Nop.Plugin.Widgets.Firework.Domain.Api
{
    /// <summary>
    /// Represents webhook request with HMAC authorization
    /// </summary>
    public interface IWebhookRequest
    {
        /// <summary>
        /// Gets or sets the content cignature
        /// </summary>
        public string ContentSignature { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public string EventType { get; }
    }
}