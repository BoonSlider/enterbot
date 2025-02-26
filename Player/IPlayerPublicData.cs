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
}