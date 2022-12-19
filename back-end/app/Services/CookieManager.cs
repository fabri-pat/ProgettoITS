namespace app.Services
{
    public static class CookieService
    {
        private const int ACCESS_TOKEN_COOKIE_DURATION_HOURS = 2;
        private const bool ACCESS_TOKEN_HTTP_ONLY_FLAG = true;
        private const bool ACCESS_TOKEN_SECURE_FLAG = true;
        private const SameSiteMode ACCESS_TOKEN_SAME_SITE_MODE = SameSiteMode.None;

        private const int REFRESH_TOKEN_COOKIE_DURATION_DAYS = 7;
        private const bool REFRESH_TOKEN_HTTP_ONLY_FLAG = true;
        private const bool REFRESH_TOKEN_SECURE_FLAG = true;
        private const SameSiteMode REFRESH_TOKEN_SAME_SITE_MODE = SameSiteMode.None;


        public static void SetAccessTokenResponseCookie(HttpContext context, string accessToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = ACCESS_TOKEN_HTTP_ONLY_FLAG,
                Expires = DateTime.Now.AddHours(ACCESS_TOKEN_COOKIE_DURATION_HOURS),
                Secure = ACCESS_TOKEN_SECURE_FLAG,
                SameSite = ACCESS_TOKEN_SAME_SITE_MODE
            };

            context.Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        }

        public static void SetRefreshTokenResponseCookie(HttpContext context, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = REFRESH_TOKEN_HTTP_ONLY_FLAG,
                Expires = DateTime.Now.AddDays(REFRESH_TOKEN_COOKIE_DURATION_DAYS),
                Secure = REFRESH_TOKEN_SECURE_FLAG,
                SameSite = REFRESH_TOKEN_SAME_SITE_MODE
            };

            context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        public static void SetResponseCookies(HttpContext context, string accessToken, string refreshToken)
        {
            SetAccessTokenResponseCookie(context, accessToken);

            SetRefreshTokenResponseCookie(context, refreshToken);
        }
    }
}