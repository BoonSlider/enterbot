using Player;

namespace Bots;

public class HumanBot(AutomationSettings a) : IBot(-1)
{
    public override string NamePrefix => "human";

    public override void PlayTurn(IPlayer p)
    {
        if (!a.IsActive) return;
        if (a.EducationLimit is { } edu)
            Common.AllMovesEducation(p, edu);
        if (a.MafiaLimit is { } mlim)
            Common.AllMovesMobsters(p, mlim, a.MoveReserve);
        if (a.GuardsLimit is { } glim)
            Common.AllMovesGuards(p, glim, a.MoveReserve);
        if (a.AttackLevel is { } atklvl)
            Common.MaximizeAtkLvl(p, atklvl);
        if (a.DefenseLevel is { } deflvl)
            Common.MaximizeDefLvl(p, deflvl);
        if (a.HouseLevel is { } houseLevel)
            Common.MaximizeHouseLvl(p, houseLevel);
        foreach (var w in a.WeaponLimits.Keys.ToList())
        {
            if (a.WeaponLimits[w] is { } wl)
            {
                Common.AllMovesWeapon(p, w, wl, a.MoveReserve);
            }
        }

        foreach (var vic in p.GetAllPlayers())
        {
            if (vic != p.Id && a.ShouldAttackPlayer[vic])
            {
                var wantCash = (a.PlayerAttackReq[vic] ?? a.AttackMoneyDefault);
                while (p.GetPlayerData(vic).GetCash() >= wantCash)
                {
                    var res = Common.SafeAttackPlayer(p, vic, false);
                    if (res is null)
                    {
                        break;
                    }

                    if (res is IAttackResult { AttackSucceeded: false })
                    {
                        break;
                    }
                }
            }
        }
    }
}