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
        Common.MaximizeHouseLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeAtkLvl(p);
    }
}