namespace Player;

public class Consts
{
    public const long EduCost = 250;
    public const long EduRate = 2;
    public const long JobCount = 24;
    public const long MaxJob = JobCount - 1;
    public const int MaxAtkDefLvl = 125;
    public const int MaxHouseLvl = 48;
    public const int MobsterMoves = 3;
    public const int MobsterFood = 9;
    public const int MobsterPrice = 180;
    public const int GuardMoves = 3;
    public const int GuardFood = 7;
    public const int GuardPrice = 150;
    public const int GuardTax = 2;
    public const int AtkMoves = 20;
    public const int FoodPrice = 5;
    public const int MinimumMobstersToAttack = 20;
    public const int BuyWeaponMoves = 2;

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
   
    public static readonly Dictionary<Weapon, long> WeaponDef = new()
    {
        { Weapon.Bat, 56 },
        { Weapon.Knife, 47 },
        { Weapon.Axe, 56 },
        { Weapon.Armor, 85 },
        { Weapon.Pistol, 51 },
        { Weapon.Uzi, 41 },
    };

    public static readonly Dictionary<Weapon, long> WeaponAtk = new()
    {
        { Weapon.Bat, 24 },
        { Weapon.Knife, 45 },
        { Weapon.Axe, 46 },
        { Weapon.Armor, 26 },
        { Weapon.Pistol, 67 },
        { Weapon.Uzi, 82 },
    };

    public static readonly Dictionary<Weapon, long> WeaponPrice = new()
    {
        { Weapon.Bat, 3600 },
        { Weapon.Knife, 5200 },
        { Weapon.Axe, 9500 },
        { Weapon.Armor, 11200 },
        { Weapon.Pistol, 12600 },
        { Weapon.Uzi, 18600 },
    };

}