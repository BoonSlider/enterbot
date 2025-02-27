using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class TextProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private Dictionary<string, string> _translations;

    public TextProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        var response = await _httpClient.GetStringAsync($"localization/app.et.json?v={Guid.NewGuid()}");
        _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(response)!;
    }

    public string GetText(string key)
    {
        if (_translations != null && _translations.ContainsKey(key))
        {
            return _translations[key];
        }

        return key; // Return the key itself if not found in the dictionary
    }
}