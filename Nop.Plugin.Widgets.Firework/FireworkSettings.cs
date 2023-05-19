using System;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Firework
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class FireworkSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets the merchant email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the OAuth app identifier
        /// </summary>
        public string OAuthAppId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the business identifier
        /// </summary>
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the business store identifier
        /// </summary>
        public string BusinessStoreId { get; set; }

        /// <summary>
        /// Gets or sets the HMAC secret
        /// </summary>
        public string HmacSecret { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expire date of a refresh token
        /// </summary>
        public DateTime? RefreshTokenExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the expire date of an access token
        /// </summary>
        public DateTime? TokenExpiresIn { get; set; }

        #region Advanced

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out
        /// </summary>
        public int? RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log debug messages
        /// </summary>
        public bool LogDebugInfo { get; set; }

        #endregion
    }
}