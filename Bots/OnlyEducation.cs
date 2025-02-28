using Player;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "tarkpea";

    public override Task PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        Common.AllMovesEducation(p);
        Common.AllMovesMobsters(p, null, 0);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
        if (d is { HouseLevel: >= 26, DefLevel: Consts.MaxAtkDefLvl })
        {
            Common.MaximizeAtkLvl(p);
        }

        return Task.CompletedTask;
    }
}