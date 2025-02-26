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

    public static void MaximizeAtkLvl(IPlayer p)
    {
        var d = p.MyData;
        var canAtkLvl = Calc.MaxAffordableAtkLvl(d);
        if (canAtkLvl != d.AtkLevel)
            p.UpdateAtkLevel(canAtkLvl).AssertOk();
    }

    public static void MaximizeDefLvl(IPlayer p)
    {
        var d = p.MyData;
        var canDefLvl = Calc.MaxAffordableDefLvl(d);
        if (canDefLvl != d.DefLevel)
            p.UpdateDefLevel(canDefLvl).AssertOk();
    }

    public static void MaximizeHouseLvl(IPlayer p)
    {
        var d = p.MyData;
        var canHouseLvl = Calc.MaxHouseThatCanBeBought(d);
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

    public static void AllMovesMobsters(IPlayer p, long keepMoves = 0)
    {
        var d = p.MyData;
        var moves = Math.Max(0L, d.Moves - keepMoves);
        var movesAllow = moves / Consts.MobsterMoves;
        EnsureFood(p, movesAllow * Consts.MobsterFood);
        var canHire = Calc.CanHireMobsters(d);
        canHire = Math.Min(canHire, movesAllow);
        if (canHire == 0) return;
        p.HireMobsters(canHire).AssertOk();
    }

    public static void AllMovesGuards(IPlayer p)
    {
        var d = p.MyData;
        var movesAllow = d.Moves / Consts.GuardMoves;
        EnsureFood(p, movesAllow * Consts.GuardFood);
        var canHire = Calc.CanHireGuards(d);
        if (canHire == 0) return;
        p.HireGuards(canHire).AssertOk();
    }

    public static void AttackRandomPlayer(IPlayer p)
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

            p.AttackPlayer(players[target], false);
        }
    }

    public static void AllMovesWeapon(IPlayer p, Weapon w, long? max = null)
    {
        var d = p.MyData;
        var canBuy = Calc.CanBuyWeapon(d, w);
        if (max != null)
        {
            canBuy = Math.Min(canBuy, Math.Max(0L, max.Value - d.Weapons[w]));
        }
        if (canBuy > 0)
            p.BuyWeapons(new Dictionary<Weapon, long> { [w] = canBuy }).AssertOk();
    }
}