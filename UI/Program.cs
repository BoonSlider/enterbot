using System.Globalization;
using GameEngine;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using TG.Blazor.IndexedDB;
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
builder.Services.AddSingleton<AlertTextProvider>();
builder.Services.AddSingleton<TextProvider>();
builder.Services.AddSingleton<IndexedDbService>();
builder.Services.AddSingleton<DbHelper>();
builder.Services.AddSingleton<AutomationService>();
builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();
var auto = host.Services.GetRequiredService<AutomationService>();
var engine = host.Services.GetRequiredService<Engine>();
DbHelper.Db = host.Services.GetRequiredService<IndexedDbService>();
var textProvider = host.Services.GetRequiredService<TextProvider>();
await textProvider.InitializeAsync();

var culture = new CultureInfo("et-EE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

engine.AddBots();
await engine.InitializeAsync();

await host.RunAsync();
auto.InitializeWith(engine.HumanPlayer.GetAllPlayers());
await auto.LoadAsync();
