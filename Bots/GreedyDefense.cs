using Player;

namespace Bots;

public class GreedyDefense(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "shield";

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.AllMovesGuards(p,null, 0));
        Common.MaximizeHouseLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeAtkLvl(p);
        return Task.CompletedTask;
    }
}