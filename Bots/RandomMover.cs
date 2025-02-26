using Player;

namespace Bots;

public class RandomMover(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "rando";
    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < Constants.AtkMoves)
            return;
        var moveType = Common.Rng.Next(1, 4);
        switch (moveType)
        {
            case 1:
                Common.AllMovesEducation(p);
                break;
            case 2:
                Common.AllMovesMobsters(p);
                break;
            case 3:
                Common.AllMovesGuards(p);
                break;
            case 4:
                Common.AttackRandomPlayer(p);
                break;
        }
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}