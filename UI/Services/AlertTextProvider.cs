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

    public TextProvider TextProvider
    {
        get { return _textProvider; }
    }

    public AlertMessage GetMessageText(IOperationResult res)
    {
        if (res is IAttackResult attackResult)
            return GetAttackResultText(attackResult);
        return new AlertMessage(_textProvider.GetEnumTranslation(res.Type));
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

            var weaponsText = _textProvider.GetText("WeaponsStolen");
            var first = true;
            foreach (var (w,amount) in r.WeaponsStolen)
            {
                if (amount > 0)
                {
                    weaponsText += first ? " " : ", ";
                    weaponsText += $"{_textProvider.GetEnumTranslation(w)} {amount}";
                    first = false;
                }
            }

            weaponsText += ".";
            if (!first)
            {
                notes.Add(weaponsText);
            }

            return new AlertMessage(string.Join(" ", notes));
        }

        return new AlertMessage(string.Format(_textProvider.GetText("AttackFailed"), r.MenLost)) { OverrideSuccess = false };
    }
}