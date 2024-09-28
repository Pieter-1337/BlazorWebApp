using BlazorWebApp.Dtos;
using BlazorWebApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorWebApp.Infrastructure
{
    public class BffAuthenticationStateProvider: AuthenticationStateProvider 
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager _navigationManager;
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly ICookieProvider _cookieProvider;


        public BffAuthenticationStateProvider(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor httpContextAccessor,
            NavigationManager navigationManager,
            IUserInfoProvider userInfoProvider,
            ICookieProvider cookieProvider
            )
        {
            _httpClient = httpClientFactory.CreateClient("bffClient");
            _httpContextAccessor = httpContextAccessor;
            _navigationManager = navigationManager;
            _userInfoProvider = userInfoProvider;
            _cookieProvider = cookieProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var ClaimsPrincipal = _httpContextAccessor.HttpContext.User;
            if (ClaimsPrincipal == null || !ClaimsPrincipal.Identity.IsAuthenticated)
            {
                return await GetAuthenticationState();
            }
            else
            {
                //Add some cookie/claims expiration stuff here and check in if it ok to still return current claimsPrincipal
                if (true)
                {
                    return new AuthenticationState(ClaimsPrincipal);
                }

                this._userInfoProvider.ClearUser();
                return await GetAuthenticationState();
            }
                
        }

        private async Task<AuthenticationState> GetAuthenticationState()
        {
            var cookie = _cookieProvider.GetCookie(Constants.BffCookieName);
            if (cookie != null)
            {
                //Get Id token info from IDP through BFF
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/userinfo");
                _cookieProvider.AddCookieToRequestHeaders(cookie, requestMessage.Headers);

                var response = await _httpClient.SendAsync(requestMessage);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && jsonResponse != "undefined")
                {
                    // User is authenticated, parse the response to get claims or identity
                    var claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                    var claims = new List<Claim>();

                    foreach (var claim in claimsDict)
                    {
                        claims.Add(new Claim(type: claim.Key, value: claim.Value));
                        if (claim.Key == "given_name")
                        {
                            claims.Add(new Claim(type: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", value: claim.Value));
                        }
                        Console.WriteLine($"{claim.Key}: {claim.Value}");
                    }
                    var userClaims = claims;// parse response to claims (usually a JSON format)
                    var identity = new ClaimsIdentity(userClaims, "Bff");
                    var user = new ClaimsPrincipal(identity);
                    //Fetch extra userInfo from our application and propagate to userInfoProvider
                    var GetUserInfoRequest = new HttpRequestMessage(HttpMethod.Get, "api/users/info");
                    _cookieProvider.AddCookieToRequestHeaders(cookie, GetUserInfoRequest.Headers);

                    var userInfoResponse = await _httpClient.SendAsync(GetUserInfoRequest);
                    var jsonUserInfoResponse = await userInfoResponse.Content.ReadAsStringAsync();
                    if (userInfoResponse.IsSuccessStatusCode)
                    {
                        var userInfoDto = JsonSerializer.Deserialize<UserInfoDto>(jsonUserInfoResponse);
                        this._userInfoProvider.SetUser(userInfoDto); 
                    }
                    else
                    {
                        //User got authenticated but does not exist in our Database of the app, case not handled in this demo..., but should create user in DB based on info from BFF...
                    }

                    return new AuthenticationState(user);
                }

                //Clear cookies in blazor app
                _httpContextAccessor.HttpContext?.Response?.Cookies.Delete(cookie.Name);
                //Clear user in provider
                this._userInfoProvider.ClearUser();
                //Logout at BFF, this will stop the session in the BFF and the IDP we are using for this user.
                _navigationManager.NavigateTo("https://localhost:44305/Logout", true);
            }
            // User is not authenticated
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        } 

        // You can also trigger a re-evaluation of the authentication state if needed
        public void NotifyUserChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
