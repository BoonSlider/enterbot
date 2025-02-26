namespace Player;

public interface IPlayerPrivateData
{
    public long Money { get; }
    public int JobLevel { get; }
    public IList<long> JobExp { get; }
    public long Moves { get; }
    public int AtkLevel { get; }
    public long Mobsters { get; }
    public long Guards { get; }
    public int DefLevel { get; }
    public long Fame { get; }
    public IDictionary<MoonshineItem, long> MoonshineItemCounts { get; }
    public IDictionary<GymStat, long> GymStats { get; }
}