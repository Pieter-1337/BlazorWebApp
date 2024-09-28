using BlazorWebApp.Dtos;
using BlazorWebApp.Infrastructure.Interfaces;
using System.Net.Http.Headers;

namespace BlazorWebApp.Infrastructure
{
    public class CookieProvider: ICookieProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        public CookieProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public CookieDto GetCookie(string name)
        {
            var cookieExists =_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(name, out string cookie);

            if (cookieExists)
            {
                return new CookieDto { Name = name, Value = cookie };
            }
            return null;
        }

        public HttpRequestHeaders AddCookieToRequestHeaders(CookieDto cookie, HttpRequestHeaders headers)
        {
            headers.Add("Cookie", $"{cookie.Name}={cookie.Value}");
            return headers;
        }
    }
}
