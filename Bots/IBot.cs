using Player;

namespace Bots;

public abstract class IBot(int nameSuffix)
{
    public int NameSuffix { get; set; } = nameSuffix;
    public abstract string NamePrefix { get; }
    public string Name => $"{NamePrefix}_{NameSuffix}";
    public abstract void PlayTurn(IPlayer p);
}