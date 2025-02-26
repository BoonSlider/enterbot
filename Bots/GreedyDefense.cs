using Player;

namespace Bots;

public class GreedyDefense(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "shield";

    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        Common.EnsureFood(p, 1000);
        var canHireGuards = Calculator.CanHireGuards(d);
        if (canHireGuards > 0)
            p.HireGuards(canHireGuards).AssertOk();
        var canHouseLvl = Calculator.MaxHouseThatCanBeBought(d);
        if (canHouseLvl != d.HouseLevel)
            p.BuyHouse(canHouseLvl).AssertOk();
        var canDefLvl = Calculator.MaxAffordableDefLvl(d);
        if (canDefLvl != d.DefLevel)
            p.UpdateDefLevel(canDefLvl).AssertOk();
        var canAtkLvl = Calculator.MaxAffordableAtkLvl(d);
        if (canAtkLvl != d.AtkLevel)
            p.UpdateAtkLevel(canAtkLvl).AssertOk();
    }
}