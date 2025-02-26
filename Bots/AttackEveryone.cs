using Player;

namespace Bots;

public class AttackEveryone(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "sarimõrvar";
    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < 100)
        {
            return;
        }

        Common.AllMovesMobsters(p,20);
        Common.AttackRandomPlayer(p);
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}