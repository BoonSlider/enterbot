using Microsoft.Extensions.Localization;
using Player;

namespace UI.Services;

public class MessageProvider(IStringLocalizer<App> localizer)
{
    public string GetMessageText(IOperationResult res)
    {
        return GetEnumTranslation(res.Type);
    }
    public string GetEnumTranslation<T>(T enumValue) where T : Enum
    {
        var key = $"{typeof(T).Name}_{enumValue}";
        return localizer[key].Value;
    }
}