using GameEngine;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

var host = builder.Build();
var engine = host.Services.GetRequiredService<Engine>();
engine.AddBots();
await engine.InitializeAsync();
await host.RunAsync();
