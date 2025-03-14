using Player.House;
using Player.Job;

namespace Player;

public static class Calc
{
    public static long HowManyMovesCanSpendOnEdu(IPlayerData d, long? maxEdu, long leaveMoves)
    {
        var ret = Math.Min(d.Money / Consts.EduCost, Math.Max(d.Moves - leaveMoves, 0));
        var eduLeft = Math.Max((maxEdu ?? MaxEducation) - d.Education, 0L);
        var eduLeftMoves = (eduLeft + Consts.EduRate - 1) / Consts.EduRate;
        return Math.Min(ret, eduLeftMoves);
    }

    public static long PlayerGangTotalAtk(IPlayerData playerData)
    {
        return PlayerSoloTotalAtk(playerData) + 0;
    }

    public static long PlayerGangTotalDef(IPlayerData playerData)
    {
        return PlayerSoloTotalDef(playerData) + 0;
    }

    public static long PlayerSoloTotalAtk(IPlayerData playerData)
    {
        var ret = playerData.AtkLevel * playerData.Mobsters;
        foreach (var stat in Enum.GetValues<GymStat>())
        {
            ret += Consts.GymStatAtk[stat] * playerData.GymStats[stat];
        }

        foreach (var stat in Enum.GetValues<Weapon>())
        {
            ret += Consts.WeaponAtk[stat] * playerData.Weapons[stat];
        }

        return ret;
    }

    public static long PlayerSoloTotalDef(IPlayerData playerData)
    {
        var ret = playerData.DefLevel * playerData.Guards;
        foreach (var stat in Enum.GetValues<GymStat>())
        {
            ret += Consts.GymStatDef[stat] * playerData.GymStats[stat];
        }

        foreach (var stat in Enum.GetValues<Weapon>())
        {
            ret += Consts.WeaponDef[stat] * playerData.Weapons[stat];
        }

        return ret;
    }

    public static long CanHireMobsters(IPlayerData playerData)
    {
        var foodMax = playerData.Food / Consts.MobsterFood;
        var movesMax = playerData.Moves / Consts.MobsterMoves;
        var moneyMax = playerData.Money / Consts.MobsterPrice;
        return Math.Min(foodMax, Math.Min(movesMax, moneyMax));
    }

    public static long CanHireGuards(IPlayerData playerData)
    {
        var foodMax = playerData.Food / Consts.GuardFood;
        var movesMax = playerData.Moves / Consts.GuardMoves;
        var moneyMax = playerData.Money / Consts.GuardPrice;
        return Math.Min(foodMax, Math.Min(movesMax, moneyMax));
    }

    public static int GetMaxJobLevel(IPlayerData d)
    {
        var index = 1;
        while (index + 1 < Consts.JobCount && Jobs.GetJobData(index + 1).RequiredEducation <= d.Education)
        {
            ++index;
        }

        return index;
    }

    public static long MaxProtectedGuards(IPlayerData playerData)
    {
        return Houses.GetHouseData(playerData.HouseLevel).ProtectedGuards;
    }

    public static long GetHouseCumulativePrices(long from, long to)
    {
        var cost = 0L;
        for (var lvl = from; lvl <= to; ++lvl)
            cost += Houses.GetHouseData(lvl).Price;
        return cost;
    }

    public static long MaxHouseThatCanBeBought(IPlayerData p, int maxLvl = Consts.MaxHouseLvl)
    {
        var nxt = p.HouseLevel + 1;
        var moneyLeft = p.Money;
        while (true)
        {
            if (nxt > maxLvl)
                break;
            var houseData = Houses.GetHouseData(nxt);
            if (houseData.RequiredFame > p.Fame)
                break;
            if (houseData.Price > moneyLeft)
                break;
            moneyLeft -= houseData.Price;
            ++nxt;
        }

        return nxt - 1;
    }

    public static long GetBankLimit(IPlayerData p)
    {
        return Jobs.GetJobData(p.JobLevel).BankLimit;
    }

    public static long GetUnprotectedGuards(IPlayerData victim)
    {
        var underProtection = Houses.GetHouseData(victim.HouseLevel).ProtectedGuards;
        return Math.Max(0L, victim.Guards - underProtection);
    }

    public static int MaxAffordableAtkLvl(IPlayerData d, int maxLevel = Consts.MaxAtkDefLvl)
    {
        var money = d.Money;
        var setTo = d.AtkLevel;
        for (var lvl = d.AtkLevel + 1; lvl <= maxLevel; ++lvl)
        {
            var price = Levels.LevelPrices[lvl];
            if (money >= price)
            {
                money -= price;
                setTo = lvl;
            }
            else
            {
                break;
            }
        }

        return setTo;
    }

    public static bool CanAttack(IPlayerData d)
    {
        return d is { Mobsters: >= Consts.MinimumMobstersToAttack, Moves: >= Consts.AtkMoves };
    }

    public static long MaxEducation => Jobs.GetJobData(Consts.MaxJob).RequiredEducation;

    public static int MaxAffordableDefLvl(IPlayerData d, int maxLvl = Consts.MaxAtkDefLvl)
    {
        var money = d.Money;
        var setTo = d.DefLevel;
        for (var lvl = d.DefLevel + 1; lvl <= maxLvl; ++lvl)
        {
            var price = Levels.LevelPrices[lvl];
            if (money >= price)
            {
                money -= price;
                setTo = lvl;
            }
            else
            {
                break;
            }
        }

        return setTo;
    }

    public static long CanBuyWeapon(IPlayerData d, Weapon w, long keepMoves)
    {
        var moneyAllows = d.Money / Consts.WeaponPrice[w];
        var movesAllow = Math.Max(d.Moves - keepMoves, 0L) / Consts.BuyWeaponMoves;
        return Math.Min(moneyAllows, movesAllow);
    }

    public static long TotalWeapons(IPlayerData playerData)
    {
        var total = 0L;
        foreach (var w in playerData.Weapons.Keys.ToList())
        {
            total += playerData.Weapons[w];
        }

        return total;
    }

    public static long GetFreeWeapons(IPlayerData d, Weapon w)
    {
        var guarded = Consts.WeaponGuardedRate * (d.Guards + d.Mobsters);
        var got = d.Weapons[w];
        var free = Math.Max(got - guarded, 0L);
        return free;
    }

    public static long? NextHouseLvlPrice(IPlayerData d)
    {
        if (d.HouseLevel < Consts.MaxHouseLvl)
        {
            return Houses.GetHouseData(d.HouseLevel + 1).Price;
        }

        return null;
    }

    public static long? NextAtkLvlPrice(IPlayerData d)
    {
        if (d.AtkLevel < Consts.MaxAtkDefLvl)
        {
            return Levels.LevelPrices[d.AtkLevel + 1];
        }

        return null;
    }

    public static long? NextDefLvlPrice(IPlayerData d)
    {
        if (d.DefLevel < Consts.MaxAtkDefLvl)
        {
            return Levels.LevelPrices[d.DefLevel + 1];
        }

        return null;
    }

    public static long GetMaxGuardedWeapons(IPlayerData d)
    {
        return (d.Mobsters + d.Guards) * Consts.WeaponGuardedRate;
    }

    public static bool AllLevelsMaxed(IPlayerData d)
    {
        return d is { AtkLevel: Consts.MaxAtkDefLvl, DefLevel: Consts.MaxAtkDefLvl, HouseLevel: Consts.MaxHouseLvl };
    }

    public static long GetMaxProtectedGuards(IPlayerData d)
    {
        return Houses.GetHouseData(d.HouseLevel).ProtectedGuards;
    }

    public static long TotalWeaponsStolen(IAttackResult atk)
    {
        return atk.WeaponsStolen.Values.Sum();
    }

    public static HouseData? GetNextHouse(IPlayerData d)
    {
        if (d.HouseLevel == Consts.MaxHouseLvl)
            return null;
        return Houses.GetHouseData(d.HouseLevel + 1);
    }

    public static HouseData CurrentHouse(IPlayerData d)
    {
        return Houses.GetHouseData(d.HouseLevel);
    }

    public static long MaxMoonshineCanMake(IPlayerData d)
    {
        long low = 0L, high = (long)1e15, mid;
        while (low < high)
        {
            mid = (low + high + 1) / 2;
            if (CanMakeMoonshine(d, mid))
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    private static bool CanMakeMoonshine(IPlayerData d, long amount)
    {
        long totCost = 0;
        foreach (var (key, value) in Consts.MoonshineRequirements)
        {
            long needMore = Math.Max(0L, amount * value - d.MoonshineItemCounts[key]);
            totCost += needMore * Consts.MoonshinePrices[key];
        }

        return totCost < d.Money;
    }
    
    public static readonly long MoonshineProfit = Consts.MoonshinePrices[MoonshineItem.Puskar] - Consts.MoonshineCost;
}