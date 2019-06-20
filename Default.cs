namespace Nop.Plugin.Misc.DataMesh
{
    /// <summary>
    /// Represents constants of the oAuth2.0 authentication method
    /// </summary>
    public class Default
    {
        /// <summary>
        /// System name of the oAuth2.0 authentication method
        /// </summary>
        public static string SystemName => "Misc.DataMesh";

        /// <summary>
        /// The logical name of authentication scheme
        /// </summary>
        public static string AuthenticationScheme => "IdSrv";

        /// <summary>
        /// The issuer that should be used for any claims that are created
        /// </summary>
        public static string ClaimsIssuer => "https://account.justpruvit.com";

        /// <summary>
        /// The name of the access token
        /// </summary>
        public static string AccessTokenName => "access_token";

        /// <summary>
        /// The claim type of the avatar
        /// </summary>
        public static string AvatarClaimType => "picture";

        /// <summary>
        /// Callback path
        /// </summary>
        public static string CallbackPath => "/signin-oauth2";

        /// <summary>
        /// Name of the view component
        /// </summary>
        public const string ViewComponentName = "DataMesh";
    }
}