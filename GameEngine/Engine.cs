using Bots;
using Player;
using Player.Job;

namespace GameEngine;

public class Engine
{
    private readonly LocalStorageService _storage;
    private readonly ChangeNotifier _changes;
    private readonly IndexedDbService _indexedDbService;
    private const string HumanPlayerId = "mina";
    public IPlayer HumanPlayer => _humanPlayer;
    private readonly Player _humanPlayer;
    private readonly Data _data = new();
    private readonly List<BotWithData> _bots = [];

    public Engine(LocalStorageService storage, ChangeNotifier changes, IndexedDbService indexedDbService)
    {
        _storage = storage;
        _changes = changes;
        _indexedDbService = indexedDbService;
        _data.AddPlayer(HumanPlayerId);
        _humanPlayer = new Player(HumanPlayerId, _data);
    }

    public void AddBots()
    {
        List<IBot> botList =
        [
            new DoNothing(1),
            new OnlyEducation(1),
            new GreedyDefense(1),
            new AttackEveryone(1),
            new RandomMover(1),
            new ArmorHoarder(1),
            new ClaudeBot(1),
            new BalancedMafiaBoss(1),
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

    public async Task HumanEndTurn(bool notifyChanges, AutomationSettings a)
    {
        var humanBot = new HumanBot(a);
        await humanBot.PlayTurn(_humanPlayer);
        
        foreach (var bot in _bots.OrderBy(_ => Random.Shared.Next()))
        {
            await bot.Strategy.PlayTurn(bot.Player);
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

    private async Task SaveHumanPlayerAsync()
    {
        await _storage.SetItemAsync(GetStorageKey(_humanPlayer.Mut), _humanPlayer.Mut);
    }

    private static void BeginNextTurn(PlayerData p)
    {
        p.Moves += 15;
        p.TurnsPlayed += 1;
        if (p.TurnsPlayed % 96 == 0)
        {
            RunEndOfDayEvents(p);
        }
        p.Money += Jobs.GetExperiencedIncome(p.JobLevel, p.JobExp[p.JobLevel]);
        p.JobExp[p.JobLevel] += 1;
    }

    private static void RunEndOfDayEvents(PlayerData p)
    {
        p.Fame -= 10;
        p.Fame = Math.Max(p.Fame, 0L);
    }

    public async Task ResetWorld()
    {
        await _data.ResetWorld(_indexedDbService);
        await SaveAll();
    }
}