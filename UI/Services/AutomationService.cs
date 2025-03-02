using System.Text.Json;
using Bots;
using Bots.HumanAutomation;
using Microsoft.JSInterop;

namespace UI.Services;

public class AutomationService
{
    private const string StorageKey = "automation-settings";
    private readonly IJSRuntime _jsRuntime;
    public AutomationSettings Settings { get; private set; } = new();

    public AutomationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task LoadAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (!string.IsNullOrEmpty(json))
        {
            Settings = JsonSerializer.Deserialize<AutomationSettings>(json) ?? new AutomationSettings();
        }
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(Settings);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public async Task ClearAsync()
    {
        Settings.IsActive = false;
        Settings.EducationLimit = null;
        Settings.MafiaLimit = null;
        Settings.GuardsLimit = null;
        Settings.HouseLevel = null;
        Settings.AttackLevel = null;
        Settings.DefenseLevel = null;
        Settings.MoveReserve = 0;
        foreach (var key in Settings.WeaponLimits.Keys.ToList())
        {
            Settings.WeaponLimits[key] = null;
        }

        Settings.AttackMoneyDefault = 0;
        foreach (var key in Settings.PlayerAttackReq.Keys.ToList())
        {
            Settings.PlayerAttackReq[key] = null;
            Settings.ShouldAttackPlayer[key] = false;
        }

        await SaveAsync();
    }

    public void InitializeWith(List<string> getAllPlayers)
    {
        foreach (var player in getAllPlayers)
        {
            Settings.PlayerAttackReq.TryAdd(player, null);
            Settings.ShouldAttackPlayer.TryAdd(player, false);
        }
    }
}