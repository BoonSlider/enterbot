using Player;

namespace Bots.Shared;

public class AtkStats
{
    private readonly Dictionary<string, long> _lastFail = new();
    private readonly Dictionary<string, long> _lastSuccess = new();
    private readonly Dictionary<string, long> _lastWeapons = new();
    private readonly Dictionary<string, long> _lastFameChange = new();
    private readonly List<string> _players = [];

    public AtkStats(string myId, IEnumerable<string> playerNames)
    {
        _players.AddRange(playerNames.Except([myId]));
    }

    public void SaveAtkResult(IAttackResult atk)
    {
        var vic = atk.Defender;
        if (atk.AttackSucceeded)
        {
            _lastSuccess[vic] = atk.TurnNumber;
            var w = Calc.TotalWeaponsStolen(atk);
            if (w > 0)
                _lastWeapons[vic] = w;
        }
        else
        {
            _lastFail[vic] = atk.TurnNumber;
        }

        _lastFameChange[vic] = atk.FameChange;
    }

    public long LastAtkWeapons(string id)
    {
        return _lastWeapons.GetValueOrDefault(id, 0);
    }

    public bool HaveFailedBefore(string id)
    {
        return _lastFail.ContainsKey(id);
    }

    public bool HaveGottenWeaponsBefore(string id)
    {
        return _lastWeapons.ContainsKey(id);
    }

    public long? GetLastFailed(string id)
    {
        var a = _lastFail.TryGetValue(id, out var v1) ? v1 : (long?)null;
        return a;
    }

    public long? GetLastAttacked(string id)
    {
        var a = GetLastFailed(id);
        var b = _lastSuccess.TryGetValue(id, out var v2) ? v2 : (long?)null;
        if (a is null) return b;
        if (b is null) return a;
        return Math.Max(a.Value, b.Value);
    }

    public List<string> HaventFailedAgainstSince(long maxAcceptableFail)
    {
        return _players.Where(p => HaveNotFailedSince(p, maxAcceptableFail)).ToList();
    }

    private bool HaveNotFailedSince(string s, long maxAcceptableFail)
    {
        if (!_lastFail.TryGetValue(s, out var lastFail))
        {
            return true;
        }

        return lastFail <= maxAcceptableFail;
    }

    public long? GetLastFameChange(string id)
    {
        return _lastFameChange.TryGetValue(id, out var value) ? value : null;
    }
}