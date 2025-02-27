using Player;
using Player.House;
using Player.Job;

namespace GameEngine;

public class Player(string id, Data data) : IPlayer
{
    public string Id { get; } = id;
    public IPlayerData MyData => data[Id];
    public IPlayerPublicData GetPlayerData(string playerId) => data[playerId];

    public List<string> GetAllPlayers()
    {
        return data.GetAllPlayers();
    }

    public PlayerData Mut => data[Id];

    public IOperationResult IncreaseEducation(long movesSpent)
    {
        var cost = movesSpent * Constants.EduCost;
        if (movesSpent <= 0)
            return OpRes.Err(MessageType.MustBePositive);
        if (Mut.Moves < movesSpent)
            return OpRes.Err(MessageType.NotEnoughMoves);
        if (Mut.Money < cost)
            return OpRes.Err(MessageType.NotEnoughMoney);
        Mut.Money -= cost;
        Mut.Moves -= movesSpent;
        Mut.Education += movesSpent * Constants.EduRate;
        return OpRes.Ok(MessageType.EducationIncreased);
    }

    public IOperationResult AcceptJob(int jobLevel)
    {
        var job = Jobs.GetJobData(jobLevel);
        if (Mut.Education < job.RequiredEducation)
            return OpRes.Err(MessageType.NotEnoughEducation);
        Mut.JobLevel = jobLevel;
        return OpRes.Ok(MessageType.GotHired);
    }

    public IOperationResult HireMobsters(long amount)
    {
        if (amount <= 0)
            return OpRes.Err(MessageType.MustBePositive);
        var needMoves = amount * Constants.MobsterMoves;
        var needMoney = amount * Constants.MobsterPrice;
        var needFood = amount * Constants.MobsterFood;
        if (Mut.Moves < needMoves)
            return OpRes.Err(MessageType.NotEnoughMoves);
        if (Mut.Money < needMoney)
            return OpRes.Err(MessageType.NotEnoughMoney);
        if (Mut.Food < needFood)
            return OpRes.Err(MessageType.NotEnoughFood);
        Mut.Mobsters += amount;
        Mut.Moves -= needMoves;
        Mut.Money -= needMoney;
        Mut.Food -= needFood;
        return OpRes.Ok(MessageType.HiredMobsters);
    }

    public IOperationResult HireGuards(long amount)
    {
        if (amount <= 0)
            return OpRes.Err(MessageType.MustBePositive);
        var needMoves = amount * Constants.GuardMoves;
        var needMoney = amount * Constants.GuardPrice;
        var needFood = amount * Constants.GuardFood;
        if (Mut.Moves < needMoves)
            return OpRes.Err(MessageType.NotEnoughMoves);
        if (Mut.Money < needMoney)
            return OpRes.Err(MessageType.NotEnoughMoney);
        if (Mut.Food < needFood)
            return OpRes.Err(MessageType.NotEnoughFood);
        Mut.Guards += amount;
        Mut.Moves -= needMoves;
        Mut.Money -= needMoney;
        Mut.Food -= needFood;
        return OpRes.Ok(MessageType.HiredGuards);
    }

    public IOperationResult UpdateAtkLevel(int desiredLevel)
    {
        if (desiredLevel == Mut.AtkLevel)
            return OpRes.Err(MessageType.AlreadyHave);
        if (desiredLevel < Mut.AtkLevel)
            return OpRes.Err(MessageType.LevelCantBeReduced);
        if (desiredLevel > Constants.MaxAtkDefLvl)
            return OpRes.Err(MessageType.LevelAlreadyMaxed);

        var cost = 0L;
        for (var lvl = Mut.AtkLevel + 1; lvl <= desiredLevel; lvl++)
        {
            cost += Levels.LevelPrices[lvl];
        }

        if (Mut.Money < cost)
            return OpRes.Err(MessageType.NotEnoughMoney);
        Mut.Money -= cost;
        Mut.AtkLevel = desiredLevel;
        return OpRes.Ok(MessageType.AtkLevelIncreased);
    }

    public IOperationResult UpdateDefLevel(int desiredLevel)
    {
        if (desiredLevel == Mut.DefLevel)
            return OpRes.Err(MessageType.AlreadyHave);
        if (desiredLevel < Mut.DefLevel)
            return OpRes.Err(MessageType.LevelCantBeReduced);
        if (desiredLevel > Constants.MaxAtkDefLvl)
            return OpRes.Err(MessageType.LevelAlreadyMaxed);

        var cost = 0L;
        for (var lvl = Mut.DefLevel + 1; lvl <= desiredLevel; lvl++)
        {
            cost += Levels.LevelPrices[lvl];
        }

        if (Mut.Money < cost)
            return OpRes.Err(MessageType.NotEnoughMoney);
        Mut.Money -= cost;
        Mut.DefLevel = desiredLevel;
        return OpRes.Ok(MessageType.DefLevelIncreased);
    }

    public IOperationResult BuyFood(long foodAmount)
    {
        return BuyFood(foodAmount, new Dictionary<MoonshineItem, long>());
    }

    public IOperationResult BuyFood(long foodAmount, Dictionary<MoonshineItem, long> moonshineItemCounts)
    {
        if (foodAmount <= 0) return OpRes.Err(MessageType.MustBePositive);
        var cost = foodAmount * Constants.FoodPrice;
        foreach (var item in moonshineItemCounts.Keys.ToList())
        {
            if (item == MoonshineItem.Puskar)
                return OpRes.Err(MessageType.MoonshineCantBeBought);
            if (moonshineItemCounts[item] <= 0)
            {
                return OpRes.Err(MessageType.MustBePositive);
            }

            cost += Constants.MoonshinePrices[item] * moonshineItemCounts[item];
        }

        if (cost <= 0) return OpRes.Err(MessageType.MustBePositive);
        if (Mut.Money < cost) return OpRes.Err(MessageType.NotEnoughMoney);
        Mut.Money -= cost;
        Mut.Food += foodAmount;
        foreach (var item in moonshineItemCounts.Keys.ToList())
        {
            Mut.MoonshineItemCounts[item] += moonshineItemCounts[item];
        }

        return OpRes.Ok(MessageType.BoughtIngredients);
    }

    public IOperationResult BuyHouse(long lvl)
    {
        if (lvl == Mut.HouseLevel)
            return OpRes.Err(MessageType.AlreadyHave);
        if (lvl < Mut.HouseLevel)
            return OpRes.Err(MessageType.LevelCantBeReduced);
        if (lvl > Constants.MaxHouseLvl)
            return OpRes.Err(MessageType.ChosenLevelAboveMax);
        if (Mut.Fame < Houses.GetHouseData(lvl).RequiredFame)
            return OpRes.Err(MessageType.NotEnoughFame);
        var cumPrice = Calc.GetHouseCumulativePrices(Mut.HouseLevel + 1, lvl);
        if (Mut.Money < cumPrice)
        {
            return OpRes.Err(MessageType.NotEnoughMoney);
        }

        Mut.HouseLevel = lvl;
        Mut.Money -= cumPrice;
        return OpRes.Ok(MessageType.BoughtNewBuilding);
    }

    public IOperationResult AttackPlayer(string victimId, bool withGang)
    {
        if (Mut.Mobsters < Constants.MinimumMobstersToAttack)
            return OpRes.Err(MessageType.NotEnoughMobsters);
        if (Mut.Moves < Constants.AtkMoves)
            return OpRes.Err(MessageType.NotEnoughMoves);
        if (Mut.Id == victimId)
            return OpRes.Err(MessageType.StopPlayingWithYourself);
        var victim = data[victimId];
        var atk = withGang ? Calc.PlayerGangTotalAtk(MyData) : Calc.PlayerSoloTotalAtk(MyData);
        var def = Calc.PlayerGangTotalDef(victim);
        Mut.Moves -= Constants.AtkMoves;
        if (atk > def)
        {
            var cash = victim.GetCash();
            var takeRatio = (double)atk / (atk + def);
            var moneyStolen = (long)Math.Floor(cash * takeRatio);
            var unprotectedGuards = Calc.GetUnprotectedGuards(victim);
            var guardsKilled = Math.Min(atk / def, unprotectedGuards);
            Mut.Money += moneyStolen;
            victim.Money -= moneyStolen;
            victim.Guards -= guardsKilled;
            if (victim.Fame > Mut.Fame)
            {
                Mut.Fame += 2;
            }
            else
            {
                Mut.Fame += 1;
            }

            return new AttackResult
                { Success = true, AttackSucceeded = true, MoneyStolen = moneyStolen, GuardsKilled = guardsKilled };
        }

        if (victim.Fame > Mut.Fame)
        {
            Mut.Fame -= 2;
        }
        else
        {
            Mut.Fame -= 1;
        }

        Mut.Fame = Math.Max(Mut.Fame, 0L);

        var menLost = Math.Min(def / atk, Mut.Mobsters);
        Mut.Mobsters -= menLost;
        return new AttackResult { Success = true, AttackSucceeded = false, MenLost = menLost };
    }
}