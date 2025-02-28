using Player;

namespace Bots;

public static class Common
{
    public static readonly Random Rng = new();

    public static void EnsureFood(IPlayer p, long amount)
    {
        if (amount == 0) return;
        var d = p.MyData;
        var need = Math.Max(0L, amount - d.Food);
        var buy = Math.Min(need, d.Money / Consts.FoodPrice);
        if (buy == 0) return;
        p.BuyFood(buy).AssertOk();
    }

    public static void MaximizeAtkLvl(IPlayer p, int maxLvl = Consts.MaxAtkDefLvl)
    {
        var d = p.MyData;
        var canAtkLvl = Calc.MaxAffordableAtkLvl(d, maxLvl);
        if (canAtkLvl != d.AtkLevel)
            p.UpdateAtkLevel(canAtkLvl).AssertOk();
    }

    public static void MaximizeDefLvl(IPlayer p, int maxLvl = Consts.MaxAtkDefLvl)
    {
        var d = p.MyData;
        var canDefLvl = Calc.MaxAffordableDefLvl(d, maxLvl);
        if (canDefLvl != d.DefLevel)
            p.UpdateDefLevel(canDefLvl).AssertOk();
    }

    public static void MaximizeHouseLvl(IPlayer p, int maxLvl = Consts.MaxHouseLvl)
    {
        var d = p.MyData;
        var canHouseLvl = Calc.MaxHouseThatCanBeBought(d, maxLvl);
        if (canHouseLvl != d.HouseLevel)
            p.BuyHouse(canHouseLvl).AssertOk();
    }

    public static void AllMovesEducation(IPlayer p, long? maxEdu = null)
    {
        var d = p.MyData;
        var canGetEdu = Calc.HowManyMovesCanSpendOnEdu(d, maxEdu);
        if (canGetEdu > 0)
            p.IncreaseEducation(canGetEdu).AssertOk();
        var maxJob = Calc.GetMaxJobLevel(d);
        if (d.JobLevel != maxJob)
        {
            p.AcceptJob(maxJob).AssertOk();
        }
    }

    public static void AllMovesMobsters(IPlayer p, long? maxCount, long keepMoves)
    {
        var d = p.MyData;
        var moves = Math.Max(0L, d.Moves - keepMoves);
        var movesAllow = moves / Consts.MobsterMoves;
        var limAllows = maxCount == null ? movesAllow : Math.Max(maxCount.Value - d.Mobsters, 0L);
        movesAllow = Math.Min(movesAllow, limAllows);
        EnsureFood(p, movesAllow * Consts.MobsterFood);
        var canHire = Calc.CanHireMobsters(d);
        canHire = Math.Min(canHire, movesAllow);
        if (canHire == 0) return;
        p.HireMobsters(canHire).AssertOk();
    }
    
    public static void AllMovesGuards(IPlayer p, long? maxCount, long keepMoves)
    {
        var d = p.MyData;
        var moves = Math.Max(0L, d.Moves - keepMoves);
        var movesAllow = moves / Consts.GuardMoves;
        var limAllows = maxCount == null ? movesAllow : Math.Max(maxCount.Value - d.Guards, 0L);
        movesAllow = Math.Min(movesAllow, limAllows);
        EnsureFood(p, movesAllow * Consts.GuardFood);
        var canHire = Calc.CanHireGuards(d);
        canHire = Math.Min(canHire, movesAllow);
        if (canHire == 0) return;
        p.HireGuards(canHire).AssertOk();
    }


    public static async Task AttackRandomPlayer(IPlayer p)
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

            await p.AttackPlayer(players[target], false);
        }
    }

    public static void AllMovesWeapon(IPlayer p, Weapon w, long? max, long keepMoves)
    {
        var d = p.MyData;
        var canBuy = Calc.CanBuyWeapon(d, w, keepMoves);
        if (max != null)
        {
            canBuy = Math.Min(canBuy, Math.Max(0L, max.Value - d.Weapons[w]));
        }
        if (canBuy > 0)
            p.BuyWeapons(new Dictionary<Weapon, long> { [w] = canBuy }).AssertOk();
    }

    public static async Task<IOperationResult?> SafeAttackPlayer(IPlayer p, string vic, bool withGang)
    {
        var d = p.MyData;
        if (Calc.CanAttack(d))
        {
            var res = await p.AttackPlayer(vic, withGang);
            return res;
        }

        return null;
    }
}