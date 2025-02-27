using Player;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "tarkpea";

    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        Common.AllMovesEducation(p);
        Common.AllMovesGuards(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
        if (d is { HouseLevel: >= 26, DefLevel: Consts.MaxAtkDefLvl })
        {
            Common.MaximizeAtkLvl(p);
        }
    }
}