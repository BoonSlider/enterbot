using Player;

namespace Bots.Shared;

public static class Common
{
    public static readonly Random Rng = new();

    public static List<IOperationResult> EnsureFood(IPlayer p, long amount)
    {
        if(amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount == 0) return [];
        var d = p.MyData;
        var need = Math.Max(0L, amount - d.Food);
        var buy = Math.Min(need, d.Money / Consts.FoodPrice);
        if (buy <= 0) return [];
        return [p.BuyFood(buy)];
    }

    public static IList<IOperationResult> MaximizeAtkLvl(IPlayer p, int maxLvl = Consts.MaxAtkDefLvl)
    {
        var d = p.MyData;
        var canAtkLvl = Calc.MaxAffordableAtkLvl(d, maxLvl);
        if (canAtkLvl != d.AtkLevel)
            return [p.UpdateAtkLevel(canAtkLvl)];
        return [];
    }

    public static IList<IOperationResult> MaximizeDefLvl(IPlayer p, int maxLvl = Consts.MaxAtkDefLvl)
    {
        var ops = new List<IOperationResult>();
        var d = p.MyData;
        var canDefLvl = Calc.MaxAffordableDefLvl(d, maxLvl);
        if (canDefLvl != d.DefLevel)
            ops.AddRange(p.UpdateDefLevel(canDefLvl));
        return ops;
    }

    public static IList<IOperationResult> MaximizeHouseLvl(IPlayer p, int maxLvl = Consts.MaxHouseLvl)
    {
        var d = p.MyData;
        var canHouseLvl = Calc.MaxHouseThatCanBeBought(d, maxLvl);
        if (canHouseLvl != d.HouseLevel)
            return [p.BuyHouse(canHouseLvl)];
        return [];
    }

    public static List<IOperationResult> AllMovesEducation(IPlayer p, long? maxEdu, long leaveMoves)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        var canGetEdu = Calc.HowManyMovesCanSpendOnEdu(d, maxEdu, leaveMoves);
        if (canGetEdu > 0)
            ops.AddRange(p.IncreaseEducation(canGetEdu));
        var maxJob = Calc.GetMaxJobLevel(d);
        if (d.JobLevel != maxJob)
        {
            ops.AddRange(p.AcceptJob(maxJob));
        }
        return ops;
    }

    public static List<IOperationResult> AllMovesMobsters(IPlayer p, long? maxCount, long keepMoves)
    {
        var d = p.MyData;
        var moves = Math.Max(0L, d.Moves - keepMoves);
        var movesAllow = moves / Consts.MobsterMoves;
        var limAllows = maxCount == null ? movesAllow : Math.Max(maxCount.Value - d.Mobsters, 0L);
        movesAllow = Math.Min(movesAllow, limAllows);
        var ops = new List<IOperationResult>();
        ops.AddRange(EnsureFood(p, movesAllow * Consts.MobsterFood));
        var canHire = Calc.CanHireMobsters(d);
        canHire = Math.Min(canHire, movesAllow);
        if (canHire <= 0) return ops;
        ops.AddRange(p.HireMobsters(canHire));
        return ops;
    }
    
    public static IList<IOperationResult> AllMovesGuards(IPlayer p, long? maxCount, long keepMoves)
    {
        var d = p.MyData;
        var moves = Math.Max(0L, d.Moves - keepMoves);
        var movesAllow = moves / Consts.GuardMoves;
        var limAllows = maxCount == null ? movesAllow : Math.Max(maxCount.Value - d.Guards, 0L);
        movesAllow = Math.Min(movesAllow, limAllows);
        var ops = new List<IOperationResult>();
        ops.AddRange(EnsureFood(p, movesAllow * Consts.GuardFood));
        var canHire = Calc.CanHireGuards(d);
        canHire = Math.Min(canHire, movesAllow);
        if (canHire <= 0) return ops;
        ops.AddRange(p.HireGuards(canHire));
        return ops;
    }


    public static IList<IOperationResult> AttackRandomPlayer(IPlayer p)
    {
        var d = p.MyData;
        if (Calc.CanAttack(d))
        {
            var players = p.GetAllPlayers();
            var target = Common.Rng.Next(players.Count);
            while (players[target] == p.Id)
            {
                target = Rng.Next(players.Count);
            }

            return [p.AttackPlayer(players[target], false)];
        }

        return [];
    }

    public static IList<IOperationResult> AllMovesWeapon(IPlayer p, Weapon w, long? max, long keepMoves)
    {
        var d = p.MyData;
        var canBuy = Calc.CanBuyWeapon(d, w, keepMoves);
        if (max != null)
        {
            canBuy = Math.Min(canBuy, Math.Max(0L, max.Value - d.Weapons[w]));
        }
        if (canBuy > 0)
            return [p.BuyWeapons(new Dictionary<Weapon, long> { [w] = canBuy })];
        return [];
    }

    public static IAttackResult? SafeAttackPlayer(IPlayer p, string vic, bool withGang)
    {
        var d = p.MyData;
        if (Calc.CanAttack(d))
        {
            return (IAttackResult)p.AttackPlayer(vic, withGang);
        }

        return null;
    }

    public static IList<IOperationResult> AllMovesProtectedGuards(IPlayer p, long keepMoves)
    {
        var maxGuards = Calc.GetMaxProtectedGuards(p.MyData);
        return AllMovesGuards(p, maxGuards, keepMoves);
    }
}