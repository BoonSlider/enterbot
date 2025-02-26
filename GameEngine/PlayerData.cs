using Player;

namespace GameEngine;

public class PlayerData : IPlayerData
{
    public required string Id { get; init; }
    public int AtkLevel { get; set; } = 1;
    public int DefLevel { get; set; } = 1;
    public IDictionary<MoonshineItem, long> MoonshineItemCounts { get; set; } = new Dictionary<MoonshineItem, long>
    {
        { MoonshineItem.Teravili, 0 },
        { MoonshineItem.Suhkur, 0 },
        { MoonshineItem.Pärm, 0 },
        { MoonshineItem.Puskar, 0 },
    };

    public IDictionary<GymStat, long> GymStats { get; set; } = new Dictionary<GymStat, long>
    {
        { GymStat.Skill, 10 },
        { GymStat.Strength, 10 },
        { GymStat.Agility, 10 },
    };

    public IList<long> JobExp { get; set; } = Enumerable.Repeat(0L, (int)Constants.JobCount).ToList();
    public int JobLevel { get; set; } = 1;
    public long Education { get; set; } = 20;
    public long Food { get; set; } = 200;
    public long HouseLevel { get; set; } = 1;
    public long Fame { get; set; } = 0;
    public long Guards { get; set; }
    public long Mobsters { get; set; }
    public long TurnsPlayed { get; set; } = 0;
    public long Money { get; set; } = 30_000;
    public long Moves { get; set; } = 700;

    public void Update(PlayerData saved)
    {
        AtkLevel = saved.AtkLevel;
        DefLevel = saved.DefLevel;
        Education = saved.Education;
        Fame = saved.Fame;
        Food = saved.Food;
        Guards = saved.Guards;
        GymStatsSaved(saved);
        HouseLevel = saved.HouseLevel;
        JobExpSaved(saved);
        JobLevel = saved.JobLevel;
        Mobsters = saved.Mobsters;
        Money = saved.Money;
        MoonshineItemsCountSaved(saved);
        Moves = saved.Moves;
        TurnsPlayed = saved.TurnsPlayed;
    }

    private void GymStatsSaved(PlayerData saved)
    {
        foreach (var key in saved.GymStats.Keys)
        {
            GymStats[key] = saved.GymStats[key];
        }
    }

    private void MoonshineItemsCountSaved(PlayerData saved)
    {
        foreach (var key in saved.MoonshineItemCounts.Keys)
        {
            MoonshineItemCounts[key] = saved.MoonshineItemCounts[key];
        }
    }

    private void JobExpSaved(PlayerData saved)
    {
        for (var i = 0; i < Constants.JobCount; i++)
        {
            JobExp[i] = saved.JobExp[i];
        }
    }

    public long GetScore()
    {
        long score = 0;
        score += Money / 100;
        score += (AtkLevel + 3) * (DefLevel + 3) * (HouseLevel + 3) * (Guards + Mobsters + 3);
        return score;
    }

    public long GetCash()
    {
        return Math.Max(Money - Calculator.GetBankLimit(this), 0L);
    }
    public FameLevel GetFameLevel()
    {
        return Enum.GetValues(typeof(FameLevel))
            .Cast<FameLevel>()
            .OrderByDescending(level => (int)level)
            .First(level => (long)level <= Fame);
    }
}