namespace Player.Job;

public record JobData
{
    public required string JobTitle { get; init; }
    public required long Income { get; init; }
    public required long RequiredEducation { get; init; }
    public required long AllowedToBorrowPercentage { get; init; }
    public required long BankLimit { get; init; }
    public required long MaxSafeLvl { get; init; }
}