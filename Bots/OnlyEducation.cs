using Bots.Shared;
using Player;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "tarkpea";

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.AllMovesEducation(p, null, 0));
        ops.AddRange(Common.AllMovesMobsters(p, null, 0));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        if (d is { DefLevel: Consts.MaxAtkDefLvl })
        {
            ops.AddRange(Common.MaximizeAtkLvl(p));
        }

        return ops;
    }
}