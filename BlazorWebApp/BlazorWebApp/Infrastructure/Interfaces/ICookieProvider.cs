using BlazorWebApp.Dtos;
using System.Net.Http.Headers;

namespace BlazorWebApp.Infrastructure.Interfaces
{
    public interface ICookieProvider
    {
        CookieDto GetCookie(string name);

        HttpRequestHeaders AddCookieToRequestHeaders(CookieDto cookie, HttpRequestHeaders request);
    }
}
