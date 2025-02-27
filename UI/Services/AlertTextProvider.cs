using GameEngine;
using Player;
using UI;

public class AlertTextProvider
{
    private readonly TextProvider _textProvider;

    public AlertTextProvider(TextProvider textProvider)
    {
        _textProvider = textProvider;
    }

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
            var notes = new List<string> { _textProvider.GetText("AttackSucceeded") };
            if (r.MoneyStolen > 0)
            {
                notes.Add(string.Format(_textProvider.GetText("MoneyStolen"), Utils.SepThousands(r.MoneyStolen)));
            }

            if (r.GuardsKilled > 0)
            {
                notes.Add(string.Format(_textProvider.GetText("GuardsKilled"), r.GuardsKilled));
            }

            return new AlertMessage(string.Join(" ", notes));
        }

        return new AlertMessage(string.Format(_textProvider.GetText("AttackFailed"), r.MenLost)) { OverrideSuccess = false };
    }

    public string GetEnumTranslation<T>(T enumValue) where T : Enum
    {
        var key = $"{typeof(T).Name}_{enumValue}";
        return _textProvider.GetText(key);
    }
}