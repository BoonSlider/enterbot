namespace Player.House;

public record HouseData
{
    public required long Price { get; init; }
    public required long Taxes { get; init; }
    public required long ProtectedGuards { get; init; }
    public required long MoonshineLimit { get; init; }
    public required long RequiredFame { get; init; }

    public static string GetHouseName(long lvl)
    {
        return lvl switch
        {
            > 32 => $"Villa lvl {lvl - 32}",
            > 16 => $"Maja lvl {lvl - 16}",
            _ => $"Korter lvl {lvl}",
        };
    }
}