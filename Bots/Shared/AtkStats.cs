using Player;

namespace Bots.Shared;

public class AtkStats
{
    private readonly Dictionary<string, long> _lastFail = new();
    private readonly Dictionary<string, long> _lastSuccess = new();
    private readonly Dictionary<string, long> _lastWeapons = new();

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
    }

    public long LastAtkWeapons(string id)
    {
        return !_lastFail.ContainsKey(id) ? 0 : _lastWeapons[id];
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
        var a = _lastFail.TryGetValue(id, out var v1) ? v1: (long?)null;
        return a;
    }
    public long? GetLastAttacked(string id)
    {
        var a = GetLastFailed(id);
        var b = _lastSuccess.TryGetValue(id, out var v2) ? v2: (long?)null;
        if (a is null) return b;
        if (b is null) return a;
        return Math.Max(a.Value, b.Value);
    }
}