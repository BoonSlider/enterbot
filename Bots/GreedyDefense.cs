using Player;

namespace Bots;

public class GreedyDefense(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "shield";

    public override void PlayTurn(IPlayer p)
    {
        Common.AllMovesGuards(p);
        Common.MaximizeHouseLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeAtkLvl(p);
    }
}