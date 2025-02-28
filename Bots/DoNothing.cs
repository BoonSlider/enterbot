using Player;

namespace Bots;

public class DoNothing(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "ei_mängi";

    public override Task PlayTurn(IPlayer p)
    {
        return Task.CompletedTask;
        // no actions
    }
}