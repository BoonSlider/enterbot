using Player;

namespace Bots;

public class SmartAtk(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "killua";
    private readonly Dictionary<string, long> _lastFail = new();
    private readonly Dictionary<string, long> _lastSuccess = new();
    private readonly Dictionary<string, long> _lastWeapons = new();
    private const long WaitBeforeRetry = 96*7;

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
        var interestingAmount = InterestingAmount(d);
        var op = new List<IOperationResult>();
        foreach (var vic in oth)
        {
            var vd = p.GetPlayerData(vic);
            while (AttackCondition(vd, interestingAmount, d))
            {
                var atk = (IAttackResult)p.AttackPlayer(vic, false);
                if (atk.AttackSucceeded)
                {
                    _lastSuccess[vic] = d.TurnsPlayed;
                    _lastWeapons[vic] = Calc.TotalWeaponsStolen(atk);
                }
                else
                {
                    _lastFail[vic] = d.TurnsPlayed;
                    _lastWeapons[vic] = 0;
                }
                op.Add(atk);
            }
        }

        return op;
    }

    private bool AttackCondition(IPlayerPublicData vd, long interestingAmount, IPlayerData d)
    {
        if (!Calc.CanAttack(d)) return false;
        var lastAttacked = GetLastAttacked(vd.Id) ?? 0;
        if (d.TurnsPlayed - lastAttacked >= GetMaxWait(vd.Id))
            return true;
        var lastFailed = GetLastFailed(vd.Id);
        if (lastFailed != null && d.TurnsPlayed - lastFailed < GetFailedWait(vd.Id))
            return false;
        var expectedWeapons = _lastWeapons.TryGetValue(vd.Id, out var lastW) ? lastW : (long?)null;
        if (expectedWeapons * Consts.BuyWeaponMoves >= Consts.AtkMoves)
            return true;
        var effectiveMoves = Consts.AtkMoves - expectedWeapons * Consts.BuyWeaponMoves;
        var cash = vd.GetCash();
        if (cash * Consts.AtkMoves >= interestingAmount * effectiveMoves)
            return true;
        return false;
    }

    private long GetMaxWait(string id)
    {
        if (_lastWeapons.ContainsKey(id) && !_lastFail.ContainsKey(id))
        {
            return WaitBeforeRetry / 20;
        }

        return WaitBeforeRetry;
    }

    private long GetFailedWait(string id)
    {
        return WaitBeforeRetry;
    }

    private long? GetLastFailed(string id)
    {
        var a = _lastFail.TryGetValue(id, out var v1) ? v1: (long?)null;
        return a;
    }
    private long? GetLastAttacked(string id)
    {
        var a = GetLastFailed(id);
        var b = _lastSuccess.TryGetValue(id, out var v2) ? v2: (long?)null;
        if (a is null) return b;
        if (b is null) return a;
        return Math.Max(a.Value, b.Value);
    }
}