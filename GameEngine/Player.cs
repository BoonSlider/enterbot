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
            return OperationResult.MustBePositive;
        if (Mut.Moves < movesSpent)
            return OperationResult.NotEnoughMoves;
        if (Mut.Money < cost)
            return OperationResult.NotEnoughMoney;
        Mut.Money -= cost;
        Mut.Moves -= movesSpent;
        Mut.Education += movesSpent * Constants.EduRate;
        return OperationResult.Ok("Oled tark nüüd.");
    }

    public IOperationResult AcceptJob(int jobLevel)
    {
        var job = Jobs.GetJobData(jobLevel);
        if (Mut.Education < job.RequiredEducation)
            return OperationResult.NotEnoughEducation;
        Mut.JobLevel = jobLevel;
        return OperationResult.Ok("Võeti tööle!");
    }

    public IOperationResult HireMobsters(long amount)
    {
        if (amount <= 0)
            return OperationResult.MustBePositive;
        var needMoves = amount * Constants.MobsterMoves;
        var needMoney = amount * Constants.MobsterPrice;
        var needFood = amount * Constants.MobsterFood;
        if (Mut.Moves < needMoves)
            return OperationResult.NotEnoughMoves;
        if (Mut.Money < needMoney)
            return OperationResult.NotEnoughMoney;
        if (Mut.Food < needFood)
            return OperationResult.NotEnoughFood;
        Mut.Mobsters += amount;
        Mut.Moves -= needMoves;
        Mut.Money -= needMoney;
        Mut.Food -= needFood;
        return OperationResult.Ok("Palkasid mafioosod.");
    }

    public IOperationResult HireGuards(long amount)
    {
        if (amount <= 0)
            return OperationResult.MustBePositive;
        var needMoves = amount * Constants.GuardMoves;
        var needMoney = amount * Constants.GuardPrice;
        var needFood = amount * Constants.GuardFood;
        if (Mut.Moves < needMoves)
            return OperationResult.NotEnoughMoves;
        if (Mut.Money < needMoney)
            return OperationResult.NotEnoughMoney;
        if (Mut.Food < needFood)
            return OperationResult.NotEnoughFood;
        Mut.Guards += amount;
        Mut.Moves -= needMoves;
        Mut.Money -= needMoney;
        Mut.Food -= needFood;
        return OperationResult.Ok("Palkasid turvamehed.");
    }

    public IOperationResult UpdateAtkLevel(int desiredLevel)
    {
        if (desiredLevel == Mut.AtkLevel)
            return OperationResult.AlreadyHave;
        if (desiredLevel < Mut.AtkLevel)
            return OperationResult.LevelCantBeReduced;
        if (desiredLevel > Constants.MaxAtkDefLvl)
            return OperationResult.LevelAlreadyMaxed;

        var cost = 0L;
        for (var lvl = Mut.AtkLevel + 1; lvl <= desiredLevel; lvl++)
        {
            cost += Levels.LevelPrices[lvl];
        }

        if (Mut.Money < cost)
            return OperationResult.NotEnoughMoney;
        Mut.Money -= cost;
        Mut.AtkLevel = desiredLevel;
        return OperationResult.Ok("Maffia level uuendatud.");
    }

    public IOperationResult UpdateDefLevel(int desiredLevel)
    {
        if (desiredLevel == Mut.DefLevel)
            return OperationResult.AlreadyHave;
        if (desiredLevel < Mut.DefLevel)
            return OperationResult.LevelCantBeReduced;
        if (desiredLevel > Constants.MaxAtkDefLvl)
            return OperationResult.LevelAlreadyMaxed;

        var cost = 0L;
        for (var lvl = Mut.DefLevel + 1; lvl <= desiredLevel; lvl++)
        {
            cost += Levels.LevelPrices[lvl];
        }

        if (Mut.Money < cost)
            return OperationResult.NotEnoughMoney;
        Mut.Money -= cost;
        Mut.DefLevel = desiredLevel;
        return OperationResult.Ok("Turva level uuendatud.");
    }

    public IOperationResult BuyFood(long foodAmount)
    {
        return BuyFood(foodAmount, new Dictionary<MoonshineItem, long>());
    }

    public IOperationResult BuyFood(long foodAmount, Dictionary<MoonshineItem, long> moonshineItemCounts)
    {
        if (foodAmount < 0) return OperationResult.MustBeNonNegative;
        var cost = foodAmount * Constants.FoodPrice;
        foreach (var item in moonshineItemCounts.Keys.ToList())
        {
            if (item == MoonshineItem.Puskar)
                return OperationResult.Error("Puskarit ei saa osta.");
            if (moonshineItemCounts[item] <= 0)
            {
                return OperationResult.MustBePositive;
            }

            cost += Constants.MoonshinePrices[item] * moonshineItemCounts[item];
        }

        if (cost <= 0) return OperationResult.MustBePositive;
        if (Mut.Money < cost) return OperationResult.NotEnoughMoney;
        Mut.Money -= cost;
        Mut.Food += foodAmount;
        foreach (var item in moonshineItemCounts.Keys.ToList())
        {
            Mut.MoonshineItemCounts[item] += moonshineItemCounts[item];
        }

        return OperationResult.Ok("Ostsid toiduaineid.");
    }

    public IOperationResult BuyHouse(long lvl)
    {
        if (lvl == Mut.HouseLevel)
            return OperationResult.AlreadyHave;
        if (lvl < Mut.HouseLevel)
            return OperationResult.LevelCantBeReduced;
        if (lvl > Constants.MaxHouseLvl)
            return OperationResult.ChosenLevelAboveMax;
        if (Mut.Fame < Houses.GetHouseData(lvl).RequiredFame)
            return OperationResult.NotEnoughFame;
        var cumPrice = Calc.GetHouseCumulativePrices(Mut.HouseLevel + 1, lvl);
        if (Mut.Money < cumPrice)
        {
            return OperationResult.NotEnoughMoney;
        }

        Mut.HouseLevel = lvl;
        Mut.Money -= cumPrice;
        return OperationResult.Ok("Ostsid uue hoone.");
    }

    public IAttackResult AttackPlayer(string victimId, bool withGang)
    {
        if (Mut.Mobsters < Constants.MinimumMobstersToAttack)
            return AttackResult.DidNotAttempt(OperationResult.NotEnoughMobsters);
        if (Mut.Moves < Constants.AtkMoves)
            return AttackResult.DidNotAttempt(OperationResult.NotEnoughMoves);
        if (Mut.Id == victimId)
            return AttackResult.DidNotAttempt(OperationResult.StopPlayingWithYourself);
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