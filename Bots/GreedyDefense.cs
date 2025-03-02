using Bots.Shared;
using Player;

namespace Bots;

public class GreedyDefense(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "shield";

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.AllMovesGuards(p,null, 0));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeAtkLvl(p));
        return ops;
    }
}