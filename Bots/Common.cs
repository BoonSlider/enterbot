using Player;

namespace Bots;

public static class Common
{
    public static void EnsureFood(IPlayer p, long amount)
    {
        var d = p.MyData;
        var need = Math.Max(0L, amount - d.Food);
        var buy = Math.Min(need, d.Money / Constants.FoodPrice);
        p.BuyFood(buy).AssertOk();
    }

    public static void MaximizeAtkLvl(IPlayer p, IPlayerData d)
    {
        var canAtkLvl = Calculator.MaxAffordableAtkLvl(d);
        if (canAtkLvl != d.AtkLevel)
            p.UpdateAtkLevel(canAtkLvl).AssertOk();
    }

    public static void MaximizeDefLvl(IPlayer p, IPlayerData d)
    {
        var canDefLvl = Calculator.MaxAffordableDefLvl(d);
        if (canDefLvl != d.DefLevel)
            p.UpdateDefLevel(canDefLvl).AssertOk();
    }

    public static void MaximizeHouseLvl(IPlayer p, IPlayerData d)
    {
        var canHouseLvl = Calculator.MaxHouseThatCanBeBought(d);
        if (canHouseLvl != d.HouseLevel)
            p.BuyHouse(canHouseLvl).AssertOk();
    }
}