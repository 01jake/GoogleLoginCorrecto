using GoogleLoginCorrecto.Client.Pages;
using GoogleLoginCorrecto.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using GoogleLoginCorrecto.Services;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DevExpress.Blazor;
var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDevExpressBlazor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7245")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); 
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddGoogleOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
   
    options.Authority = "https://accounts.google.com";
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;

    options.Scope.Add(CalendarService.Scope.Calendar);
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.GetClaimsFromUserInfoEndpoint = true;

    options.ClaimActions.Clear();
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
    options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
    options.ClaimActions.MapJsonKey("picture", "picture");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    options.ClaimActions.MapJsonKey("email_verified", "email_verified", ClaimValueTypes.Boolean);

    options.CallbackPath = "/signin-oidc";
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<GoogleCalendarService>();





var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GoogleLoginCorrecto.Client._Imports).Assembly);

app.MapGet("/signin", async context =>
{
    await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});
app.MapGet("/signout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/weather");
});

app.Run();
