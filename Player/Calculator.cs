using Player.House;
using Player.Job;

namespace Player;

public static class Calculator
{
    public static long HowManyMovesCanSpendOnEdu(IPlayerData playerData)
    {
        return Math.Min(playerData.Money / Constants.EduCost, playerData.Moves);
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
            ret += Constants.GymStatAtk[stat] * playerData.GymStats[stat];
        }

        return ret;
    }

    public static long PlayerSoloTotalDef(IPlayerData playerData)
    {
        var ret = playerData.DefLevel * playerData.Guards;
        foreach (var stat in Enum.GetValues<GymStat>())
        {
            ret += Constants.GymStatDef[stat] * playerData.GymStats[stat];
        }

        return ret;
    }

    public static long CanHireMobsters(IPlayerData playerData)
    {
        var foodMax = playerData.Food / Constants.MobsterFood;
        var movesMax = playerData.Moves / Constants.MobsterMoves;
        var moneyMax = playerData.Money / Constants.MobsterPrice;
        return Math.Min(foodMax, Math.Min(movesMax, moneyMax));
    }

    public static long? CanHireGuards(IPlayerData playerData)
    {
        var foodMax = playerData.Food / Constants.GuardFood;
        var movesMax = playerData.Moves / Constants.GuardMoves;
        var moneyMax = playerData.Money / Constants.GuardPrice;
        return Math.Min(foodMax, Math.Min(movesMax, moneyMax));
    }

    public static int GetMaxJobLevel(IPlayerData d)
    {
        var index = 1;
        while (index + 1 < Constants.JobCount && Jobs.GetJobData(index + 1).RequiredEducation <= d.Education)
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

    public static long MaxHouseThatCanBeBought(IPlayerData p)
    {
        var nxt = p.HouseLevel + 1;
        var moneyLeft = p.Money;
        while (true)
        {
            if (nxt > Constants.HouseLevels)
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

    public static long GetCash(IPlayerData p)
    {
        return Math.Max(p.Money - GetBankLimit(p), 0L);
    }

    public static long GetUnprotectedGuards(IPlayerData victim)
    {
        var underProtection = Houses.GetHouseData(victim.HouseLevel).ProtectedGuards;
        return Math.Max(0L, victim.Guards - underProtection);
    }
}