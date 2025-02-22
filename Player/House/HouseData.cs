namespace Player.House;

public record HouseData
{
    public required long Price { get; init; }
    public required long Taxes { get; init; }
    public required long ProtectedGuards { get; init; }
    public required long MoonshineLimit { get; init; }
    public required long RequiredFame { get; init; }
}