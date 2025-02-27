using System.Globalization;
using GameEngine;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using UI;
using UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<Engine>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<ChangeNotifier>();
builder.Services.AddSingleton<AlertService>();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizer<App>,StringLocalizer<App>>();
builder.Services.AddSingleton<TextProvider>();

var host = builder.Build();

var culture = new CultureInfo("et-EE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var engine = host.Services.GetRequiredService<Engine>();
engine.AddBots();
await engine.InitializeAsync();

await host.RunAsync();
