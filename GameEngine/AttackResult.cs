using Player;

namespace GameEngine;

public record AttackResult : IAttackResult
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool AttackSucceeded { get; set; }
    public long MenLost { get; set; }
    public long MoneyStolen { get; set; }
    public long GuardsKilled { get; set; }
    public bool Success { get; set; }
    public IDictionary<Weapon, long> WeaponsStolen { get; set; } = new Dictionary<Weapon, long>();

    public MessageType Type => MessageType.AttackAttempted;
}