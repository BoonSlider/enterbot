using Player;

namespace GameEngine;

public record AttackResult : IAttackResult
{
    public long Id { get; set; }
    public bool AttackSucceeded { get; set; }
    public long MenLost { get; set; }
    public long MoneyStolen { get; set; }
    public long GuardsKilled { get; set; }
    public bool Success { get; set; }
    public IDictionary<Weapon, long> WeaponsStolen { get; set; } = new Dictionary<Weapon, long>();
    public long TurnNumber { get; set; }
    public MessageType Type { get; set; } = MessageType.AttackAttempted;
    public required string? Attacker { get; init; }
    public required string? Defender { get; init; }
}