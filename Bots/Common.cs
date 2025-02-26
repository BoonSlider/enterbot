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
}