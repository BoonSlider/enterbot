using Player;

namespace Bots;

public class AutomationSettings
{
    public bool IsActive { get; set; }
    public int? EducationLimit { get; set; }
    public int? MafiaLimit { get; set; }
    public int? GuardsLimit { get; set; }
    public int? HouseLevel { get; set; }
    public int? AttackLevel { get; set; }
    public int? DefenseLevel { get; set; }
    public int MoveReserve { get; set; }

    public Dictionary<Weapon, long?> WeaponLimits { get; init; } =
        Enum.GetValues<Weapon>().ToDictionary(s => s, _ => (long?)null);

    public long AttackMoneyDefault { get; set; }
    public Dictionary<string, long?> PlayerAttackReq { get; init; } = new();
    public Dictionary<string, bool> ShouldAttackPlayer { get; init; } = new();
}