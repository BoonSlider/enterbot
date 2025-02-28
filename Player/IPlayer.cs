namespace Player;

public interface IPlayer : IPlayerActions
{
    public string? Id { get; }
    IPlayerData MyData { get; }
    IPlayerPublicData GetPlayerData(string playerId);
    public List<string> GetAllPlayers();
}