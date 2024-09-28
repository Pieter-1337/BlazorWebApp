using BlazorWebApp.Components;
using BlazorWebApp.Infrastructure;
using BlazorWebApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<ICookieProvider, CookieProvider>();
builder.Services.AddHttpClient("bffClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44305"); // Your BFF base address
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        UseCookies = true, // Enables automatic cookie handling
        CookieContainer = new CookieContainer() // Cookie container to manage cookies
    };
});
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();
builder.Services.AddScoped<IUserInfoProvider, UserInfoProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

//Map our razor components to our Server App (which is the main app that gets served
app.MapRazorComponents<App>()
    //Add the render methods Server and WebAssembly
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    //Add the assemblies that are present in the WebAssembly Client project, this makes sure that we have all of the files availalbe to be served from either the server project or the webAssembly project
    .AddAdditionalAssemblies(typeof(BlazorWebApp.Client._Imports).Assembly);



app.Run();
