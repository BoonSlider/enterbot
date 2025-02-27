namespace Player;

public interface IAttackResult : IOperationResult
{
    public bool AttackSucceeded { get; }
    public long MenLost { get; }
    public long MoneyStolen { get; }
    public long GuardsKilled { get; }
    public IDictionary<Weapon, long> WeaponsStolen { get; }

}