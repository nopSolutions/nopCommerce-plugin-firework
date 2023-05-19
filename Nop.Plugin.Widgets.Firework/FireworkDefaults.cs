using Nop.Core;

namespace Nop.Plugin.Widgets.Firework
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class FireworkDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Widgets.Firework";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the application name
        /// </summary>
        public static string ApplicationName => "nopCommerce-integration";

        /// <summary>
        /// Gets the provider name
        /// </summary>
        public static string ProviderName => "nopCommerce";

        /// <summary>
        /// Gets the API URL
        /// </summary>
        public static string SandboxApiUrl => "https://staging.fireworktv.com/";
        public static string ApiUrl => "https://fireworktv.com/";

        /// <summary>
        /// Gets the business portal iframe URL
        /// </summary>
        public static string SandboxPortalUrl => "https://business-staging.fireworktv.com/integration";
        public static string PortalUrl => "https://business.firework.com/integration";

        /// <summary>
        /// Gets the script URL
        /// </summary>
        public static string SandboxCdnUrl => "asset-staging.fwcdn1.com";
        public static string CdnUrl => "asset.fwcdn3.com";

        /// <summary>
        /// Gets the content signature header name
        /// </summary>
        //public static string HeaderContentSignature => "X-fw-content-signature";
        public static string HeaderContentSignature => "X-fw-hmac-sha512";

        /// <summary>
        /// Gets the timestamp header name
        /// </summary>
        public static string HeaderTimestamp => "X-fw-timestamp";

        /// <summary>
        /// Gets the webhook event header name
        /// </summary>
        public static string HeaderWebhookEvent => "x-fw-webhook-event";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Widgets.Firework.Configure";

        /// <summary>
        /// Gets the oauthcallback route name
        /// </summary>
        public static string OauthCallbackRouteName => "Plugin.Widgets.Firework.OauthCallback";

        /// <summary>
        /// Gets a default period (in seconds) before the request times out
        /// </summary>
        public static int RequestTimeout => 15;

        /// <summary>
        /// Gets a name of the view component to display embedded widget in public store
        /// </summary>
        public const string VIEW_COMPONENT = "FireworkWidget";
    }
}