using Player;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "tarkpea";
    public override void PlayTurn(IPlayer p)
    {
        Common.AllMovesEducation(p);
    }
}