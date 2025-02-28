using Player;

namespace Bots;

public class AttackEveryone(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "sarimõrvar";
    public override async Task PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < 100)
        {
            return;
        }

        Common.AllMovesMobsters(p,null, 20);
        await Common.AttackRandomPlayer(p);
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}