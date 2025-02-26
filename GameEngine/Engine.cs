using Bots;
using Player;
using Player.Job;

namespace GameEngine;

public class Engine
{
    private readonly LocalStorageService _storage;
    private readonly ChangeNotifier _changes;
    private const string HumanPlayerId = "mina";
    public IPlayer HumanPlayer => _humanPlayer;
    private readonly Player _humanPlayer;
    private readonly Data _data = new();
    private readonly List<BotWithData> _bots = [];

    public Engine(LocalStorageService storage, ChangeNotifier changes)
    {
        _storage = storage;
        _changes = changes;
        _data.AddPlayer(HumanPlayerId);
        _humanPlayer = new Player(HumanPlayerId, _data);
    }

    public void AddBots()
    {
        List<IBot> botList =
        [
            new DoNothing(1),
            new DoNothing(2),
            new DoNothing(3),
            new OnlyEducation(1),
            new GreedyDefense(1),
        ];
        foreach (var bot in botList)
        {
            _data.AddPlayer(bot.Name);
            _bots.Add(new BotWithData { Player = new Player(bot.Name, _data), Strategy = bot });
        }
    }

    private string GetStorageKey(PlayerData playerData)
    {
        return $"{nameof(PlayerData)}_{playerData.Id}";
    }

    public async Task InitializeAsync()
    {
        foreach (var bot in _bots)
        {
            await UpdatePlayerData(bot.Player.Mut);
        }

        await UpdatePlayerData(_humanPlayer.Mut);
        _changes.OnChangeAsync += SaveAll;
    }

    private async Task UpdatePlayerData(PlayerData playerData)
    {
        var saved = await _storage.GetItemAsync(GetStorageKey(playerData), _data[playerData.Id]);
        playerData.Update(saved);
    }

    public async Task HumanEndTurn(bool notifyChanges)
    {
        foreach (var bot in _bots.OrderBy(_ => Random.Shared.Next()))
        {
            bot.Strategy.PlayTurn(bot.Player);
        }

        foreach (var bot in _bots)
        {
            BeginNextTurn(bot.Player.Mut);
        }

        BeginNextTurn(_humanPlayer.Mut);
        if (notifyChanges)
            await _changes.Notify();
    }

    private async Task SaveAll()
    {
        foreach (var bot in _bots)
        {
            await _storage.SetItemAsync(GetStorageKey(bot.Player.Mut), bot.Player.Mut);
        }

        await SaveHumanPlayerAsync();
    }

    public async Task SaveHumanPlayerAsync()
    {
        await _storage.SetItemAsync(GetStorageKey(_humanPlayer.Mut), _humanPlayer.Mut);
    }

    private void BeginNextTurn(PlayerData p)
    {
        p.Moves += 15;
        p.Money += Jobs.GetExperiencedIncome(p.JobLevel, p.JobExp[p.JobLevel]);
        p.JobExp[p.JobLevel] += 1;
    }

    public async Task ResetWorld()
    {
        _data.ResetWorld();
        await SaveAll();
    }
}