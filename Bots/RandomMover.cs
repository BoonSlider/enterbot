using Player;

namespace Bots;

public class RandomMover(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "rando";
    public override void PlayTurn(IPlayer p)
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
                Common.AllMovesMobsters(p);
                break;
            case 2:
                Common.AttackRandomPlayer(p);
                break;
            case 3:
                Common.AllMovesGuards(p);
                break;
        }
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}