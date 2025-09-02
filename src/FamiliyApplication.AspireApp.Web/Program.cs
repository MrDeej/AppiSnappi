
using BlazorServerCommon.Notifications;
using BlazorServerCommon.Vm;
using Eiriklb.Utils;
using FamiliyApplication.AspireApp.Web;
using FamiliyApplication.AspireApp.Web.Components;
using FamiliyApplication.AspireApp.Web.Components.FamilyEvents;
using FamiliyApplication.AspireApp.Web.Components.Home;
using FamiliyApplication.AspireApp.Web.Components.Users;
using FamiliyApplication.AspireApp.Web.CosmosDb;
using FamiliyApplication.AspireApp.Web.CosmosDb.User;
using FamiliyApplication.AspireApp.Web.Databuffer;
using FamiliyApplication.AspireApp.Web.Databuffer.PeriodicServices;
using FamiliyApplication.AspireApp.Web.Notifications;
using FamiliyApplication.AspireApp.Web.Sessions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Globalization;
using System.Security.Claims;

var cultureInfo = new CultureInfo("nb-NO")
{
    NumberFormat = { CurrencySymbol = "kr" } // Set the currency symbol to "kr"
};

// Set the default culture globally
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Define supported cultures
var supportedCultures = new[] { "nb-NO" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("nb-NO")
    .AddSupportedCultures(supportedCultures)    // Use string[] for culture names
    .AddSupportedUICultures(supportedCultures); // Use string[] for UI culture names

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddAzureKeyVaultSecrets(connectionName: "familyapp-kv");

builder.Services.AddLocalization();

builder.Services.AddRadzenComponents();
IEnumerable<string>? initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');


builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                var request = context.Request;
                var host = request.Scheme + "://" + request.Host.Value;
                context.ProtocolMessage.PostLogoutRedirectUri = host;
                return Task.CompletedTask;
            }
        };


    });


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

builder.Services.AddCascadingAuthenticationState();

builder.AddServiceDefaults();


builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       .Build();

    options.AddPolicy("AllowAnonymous", policy =>
        policy.RequireAssertion(context => true));
});


builder.Services.AddFluentUIComponents();

builder.Services.AddOutputCache();

builder.Services.AddSingleton<AppDbContext>(sp =>
{
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseCosmos(
        sp.GetRequiredService<IConfiguration>()["CosmosDb:AccountEndpoint"]!,
        sp.GetRequiredService<IConfiguration>()["CosmosDb:AccountKey"]!,
        sp.GetRequiredService<IConfiguration>()["CosmosDb:DatabaseName"]!
    );

    return new AppDbContext(optionsBuilder.Options);
});

builder.Services.AddSingleton<GlobalVm>();
builder.Services.AddSingleton<GlobalBase>(provider => provider.GetRequiredService<GlobalVm>());

builder.Services.AddScoped<FamilyDtoDataService>();
builder.Services.AddScoped<UserDtoDataService>();
builder.Services.AddScoped<BlackBoardDtoDataService>();
builder.Services.AddScoped<UserNotificationManager>();

builder.Services.AddHostedService<DataInitializer>();

builder.Services.AddHostedService<DailySortService>();
builder.Services.AddHostedService<DeleteOldNotificationsService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<SessionManager>();
builder.Services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();
builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddScoped<LottieFileService>();

builder.Services.AddScoped<FamiliyApplication.AspireApp.Web.Notifications.NotificationManager>();
builder.Services.AddScoped<NotificationManagerBase>(provider => provider.GetRequiredService<FamiliyApplication.AspireApp.Web.Notifications.NotificationManager>());

builder.Services.AddScoped<IEiriklbDispatcher, EirikLbDispatcher>();

builder.Services.AddScoped<ActivitesEngine>();

builder.Services.AddScoped<FamilyEventService>();

builder.Services.AddScoped<UserWalletSaveGoal>(sp => new UserWalletSaveGoal()
{
    Id = Guid.NewGuid().ToString(),
    Amount = 0,
    CreatedAt = DateTime.Now,
    Description = "",
    ThingToSaveFor = ""
});

var app = builder.Build();
app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
   
    app.UseHsts();
}
app.UseStaticFiles(); // This must be first to allow access to static files without authentication


app.UseHttpsRedirection();
app.UseRouting();

app.UseOutputCache();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();



app.Run();
