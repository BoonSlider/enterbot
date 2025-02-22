using System.Text.Json;
using Microsoft.JSInterop;

namespace GameEngine;

public class LocalStorageService(IJSRuntime jsRuntime)
{
    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    public async Task<T> GetItemAsync<T>(string key, T defaultValue)
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        return json == null ? defaultValue : JsonSerializer.Deserialize<T>(json)!;
    }
}