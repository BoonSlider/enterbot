using Player;

namespace Bots;

public class RandomMover(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "rando";
    public override async Task PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < Consts.AtkMoves)
            return;
        var moveType = Common.Rng.Next(4);
        switch (moveType)
        {
            case 0:
                Common.AllMovesEducation(p);
                break;
            case 1:
                Common.AllMovesMobsters(p, null, 0);
                break;
            case 2:
                await Common.AttackRandomPlayer(p);
                break;
            case 3:
                Common.AllMovesGuards(p, null, 0);
                break;
        }
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}