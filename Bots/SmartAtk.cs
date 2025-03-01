using Player;

namespace Bots;

public class SmartAtk(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "killua";
    private readonly Dictionary<string, long> _lastFail = new();

    private long InterestingAmount(IPlayerData d)
    {
        var mn = 200_000;
        var mx = 10_000_000_000L;
        // var p1 = Calc.NextHouseLvlPrice(d) ?? mx;
        var p2 = Calc.NextAtkLvlPrice(d) ?? mx;
        // var p3 = Calc.NextDefLvlPrice(d) ?? mx;
        return Math.Max(p2, mn);
    }
    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var keepMoves = 40;
        var d = p.MyData;
        var op = new List<IOperationResult>();
        op.AddRange(OpportunisticAttack(p));
        var wweaponLimit = Calc.GetMaxGuardedWeapons(d);
        if (Calc.AllLevelsMaxed(d))
        {
            op.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, wweaponLimit, keepMoves));
            var guardLimit = Calc.GetMaxProtectedGuards(d);
            op.AddRange(Common.AllMovesGuards(p, guardLimit, keepMoves));
            op.AddRange(Common.AllMovesMobsters(p, null, keepMoves));
        }
        else
        {
            op.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, wweaponLimit, keepMoves));
            op.AddRange(Common.AllMovesMobsters(p, null, keepMoves));
        }
        op.AddRange(Common.MaximizeAtkLvl(p));
        if (d.Money > 10_000_000)
        {
            op.AddRange(Common.MaximizeDefLvl(p));
            op.AddRange(Common.MaximizeHouseLvl(p));
        }
        return op;
    }

    private IList<IOperationResult> OpportunisticAttack(IPlayer p)
    {
        var oth = p.GetAllPlayers().Except([p.Id]).ToList();
        var d = p.MyData;
        const long waitBeforeRetry = 96*4;
        var interestingAmount = InterestingAmount(d);
        var op = new List<IOperationResult>();
        foreach (var vic in oth)
        {
            var vd = p.GetPlayerData(vic);
            if (vd.GetCash() >= interestingAmount && Calc.CanAttack(d))
            {
                if (!_lastFail.ContainsKey(vic) || _lastFail[vic] < d.TurnsPlayed - waitBeforeRetry)
                {
                    var atk = (IAttackResult)p.AttackPlayer(vic, false);
                    if (!atk.AttackSucceeded)
                    {
                        _lastFail[vic] = d.TurnsPlayed;
                    }
                    op.Add(atk);
                }
            }
        }

        return op;
    }
}