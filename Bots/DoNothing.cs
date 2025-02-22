using Player;

namespace Bots;

public class DoNothing(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "ei_mängi";

    public override void PlayTurn(IPlayer p)
    {
        // no actions
    }
}