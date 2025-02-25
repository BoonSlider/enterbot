namespace Player;

public class Constants
{
    public const long EduCost = 250;
    public const long EduRate = 2;
    public const long JobCount = 24;
    public const int AtkDefLevels = 125;
    public const int HouseLevels = 48;
    public const int MobsterMoves = 3;
    public const int MobsterFood = 9;
    public const int MobsterPrice = 180;
    public const int GuardMoves = 3;
    public const int GuardFood = 7;
    public const int GuardPrice = 150;
    public const int GuardTax = 2;
    public const int AtkMoves = 20;
    public const int FoodPrice = 5;
    public const int MinimumMobstersToAttack = 25;

    public static readonly Dictionary<MoonshineItem, long> MoonshinePrices = new()
    {
        { MoonshineItem.Teravili, 2 },
        { MoonshineItem.Suhkur, 3 },
        { MoonshineItem.Pärm, 4 },
        { MoonshineItem.Puskar, 67 },
    };

    public static readonly Dictionary<GymStat, long> GymStatAtk = new()
    {
        { GymStat.Skill, 60 },
        { GymStat.Strength, 53 },
        { GymStat.Agility, 48 },
    };

    public static readonly Dictionary<GymStat, long> GymStatDef = new()
    {
        { GymStat.Skill, 46 },
        { GymStat.Strength, 53 },
        { GymStat.Agility, 58 },
    };
}