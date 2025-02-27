namespace Player;

public interface IPlayerPublicData
{
    public string Id { get; }
    public long Education { get; }
    public long Food { get; }
    public long HouseLevel { get; }
    public FameLevel GetFameLevel();
    public long GetScore();
    public long GetCash();
    public long MoneyStolen { get; }
    public long MoneyLost { get; }
    public long GuardsKilled { get; }
    public long GuardsLost { get; }
    public long MobstersKilled { get; }
    public long MobstersLost { get; }
    public long WeaponsStolen { get; }
    public long WeaponsLost { get; }
}