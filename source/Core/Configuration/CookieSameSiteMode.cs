namespace IdentityServer3.Core.Configuration
{
    /// <summary>
    /// Indicates if same site flag should be issued for a cookie.
    /// </summary>
    public enum CookieSameSiteMode
    {
        /// <summary>
        /// Indicates the client should send the cookie with every requests coming from any origin.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the client should send the cookie with "same-site" requests, and with "cross-site" top-level navigations.
        /// </summary>
        Lax,
        /// <summary>
        /// Indicates the client should only send the cookie with "same-site" requests.
        /// </summary>
        Strict,
    }
}