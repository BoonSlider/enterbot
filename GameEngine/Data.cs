namespace GameEngine;

public class Data
{
    private readonly Dictionary<string, PlayerData> _allPlayerData = new();

    public List<string> GetAllPlayers() => _allPlayerData.Keys.ToList();

    public void AddPlayer(string id)
    {
        if (_allPlayerData.ContainsKey(id)) throw new Exception("Already exists");
        _allPlayerData.Add(id, new PlayerData { Id = id });
    }

    public PlayerData this[string playerId]
    {
        get => _allPlayerData[playerId];
        set => _allPlayerData[playerId] = value;
    }

    public void ResetWorld()
    {
        var defaultData = new PlayerData { Id = "default" };
        foreach (var key in _allPlayerData.Keys.ToList())
        {
            _allPlayerData[key].Update(defaultData);
        }
    }
}