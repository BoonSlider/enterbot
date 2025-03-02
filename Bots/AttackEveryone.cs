using Bots.Shared;
using Player;

namespace Bots;

public class AttackEveryone(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "chihuahua";
    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        if (d.Moves < 100)
        {
            return ops;
        }
        

        ops.AddRange(Common.AllMovesMobsters(p,null, 20));
        ops.AddRange(Common.AttackRandomPlayer(p));
        ops.AddRange(Common.MaximizeAtkLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        return ops;
    }
}