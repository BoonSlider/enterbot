using Player;

namespace Bots;

public abstract class IBot(int nameSuffix)
{
    public int NameSuffix { get; set; } = nameSuffix;
    public abstract string NamePrefix { get; }
    public string Name => $"{NamePrefix}_{NameSuffix}";
    public abstract IList<IOperationResult> PlayTurn(IPlayer p);
}