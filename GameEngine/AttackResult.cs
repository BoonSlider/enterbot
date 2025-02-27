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

    public MessageType Type => MessageType.AttackAttempted;
    // public string? Message
    // {
    //     get
    //     {
    //         if(_message != null) return _message;
    //         if (!Success) throw new InvalidOperationException("Atk not attempted without any comments.");
    //         if (AttackSucceeded)
    //         {
    //             var notes = new List<string>();
    //             notes.Add("Rünne õnnestus.");
    //             if (MoneyStolen > 0)
    //             {
    //                 notes.Add($"Röövisid {Utils.SepThousands(MoneyStolen)} EEK.");
    //             }
    //
    //             if (GuardsKilled > 0)
    //             {
    //                 notes.Add($"Tapsid {GuardsKilled} turvameest.");
    //             }
    //
    //             return string.Join(" ", notes);
    //         }
    //
    //         return $"Rünne ebaõnnestus. Kaotasid {MenLost} mafioosot.";
    //     }
    // }
}