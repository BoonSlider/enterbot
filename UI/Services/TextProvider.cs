using System.Text.Json;

namespace UI.Services;

public class TextProvider
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, string> _translations=null!;

    public TextProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        var response = await _httpClient.GetStringAsync($"localization/app.et.json?v={Guid.NewGuid()}");
        _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(response)!;
    }

    public string GetText(string key)
    {
        return _translations.GetValueOrDefault(key, key); // Return the key itself if not found in the dictionary
    }

    public string GetEnumTranslation<T>(T enumValue) where T : Enum
    {
        var key = $"{typeof(T).Name}_{enumValue}";
        return this.GetText(key);
    }
}