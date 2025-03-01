using Player;

namespace Bots;

public class RandomMover(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "rando";
    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < Consts.AtkMoves)
            return [];
        var moveType = Common.Rng.Next(4);
        var ops = new List<IOperationResult>();
        switch (moveType)
        {
            case 0:
                ops.AddRange(Common.AllMovesEducation(p));
                break;
            case 1:
                ops.AddRange(Common.AllMovesMobsters(p, null, 0));
                break;
            case 2:
                ops.AddRange(Common.AttackRandomPlayer(p));
                break;
            case 3:
                ops.AddRange(Common.AllMovesGuards(p, null, 0));
                break;
        }
        ops.AddRange(Common.MaximizeAtkLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        return ops;
    }
}