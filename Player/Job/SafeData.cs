namespace Player.Job;

public record SafeData
{
    public required long Price { get; init; }
    public required long Capacity { get; init; }
    public required long Taxes { get; init; }
    public required long MinimumJobLevel { get; init; }
}