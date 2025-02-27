using GameEngine;
using Microsoft.Extensions.Localization;
using Player;

namespace UI.Services;

public class MessageProvider(IStringLocalizer<App> localizer)
{
    public AlertMessage GetMessageText(IOperationResult res)
    {
        if (res is IAttackResult attackResult)
            return GetAttackResultText(attackResult);
        return new AlertMessage(GetEnumTranslation(res.Type));
    }

    private AlertMessage GetAttackResultText(IAttackResult r)
    {
        if (r.AttackSucceeded)
        {
            var notes = new List<string> { localizer["AttackSucceeded"] };
            if (r.MoneyStolen > 0)
            {
                notes.Add(string.Format(localizer["MoneyStolen"], Utils.SepThousands(r.MoneyStolen)));
            }

            if (r.GuardsKilled > 0)
            {
                notes.Add(string.Format(localizer["GuardsKilled"], r.GuardsKilled));
            }

            return new AlertMessage(string.Join(" ", notes));
        }

        return new AlertMessage(string.Format(localizer["AttackFailed"], r.MenLost)) { OverrideSuccess = false };
    }

    private string GetEnumTranslation<T>(T enumValue) where T : Enum
    {
        var key = $"{typeof(T).Name}_{enumValue}";
        return localizer[key].Value;
    }
}