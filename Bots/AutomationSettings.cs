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

    public Dictionary<Weapon, long?> WeaponLimits { get; set; } =
        Enum.GetValues<Weapon>().ToDictionary(s => s, s => (long?)null);

    public long AttackMoneyDefault { get; set; }
    public Dictionary<string, long?> PlayerAttackReq { get; set; } = new();
}