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

    public IDictionary<Weapon, long> Weapons { get; set; } = new Dictionary<Weapon, long>
    {
        { Weapon.Bat, 0 },
        { Weapon.Knife, 0 },
        { Weapon.Axe, 0 },
        { Weapon.Armor, 0 },
        { Weapon.Pistol, 0 },
        { Weapon.Uzi, 0 },
    };

    public IList<long> JobExp { get; set; } = Enumerable.Repeat(0L, (int)Consts.JobCount).ToList();
    public int JobLevel { get; set; } = 1;
    public long Education { get; set; } = 20;
    public long Food { get; set; } = 200;
    public long HouseLevel { get; set; } = 1;
    public long Fame { get; set; }
    public long Guards { get; set; }
    public long Mobsters { get; set; }
    public long TurnsPlayed { get; set; }
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
        WeaponsSaved(saved);
        MoneyStolen = saved.MoneyStolen;
        MoneyLost = saved.MoneyLost;
        GuardsLost = saved.GuardsLost;
        GuardsKilled = saved.GuardsKilled;
        MobstersLost = saved.GuardsLost;
        MobstersKilled = saved.MobstersLost;
        WeaponsStolen = saved.WeaponsStolen;
        WeaponsLost = saved.WeaponsLost;
    }

    private void GymStatsSaved(PlayerData saved)
    {
        foreach (var key in saved.GymStats.Keys)
        {
            GymStats[key] = saved.GymStats[key];
        }
    }

    private void WeaponsSaved(PlayerData saved)
    {
        foreach (var key in saved.Weapons.Keys)
        {
            Weapons[key] = saved.Weapons[key];
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
        for (var i = 0; i < Consts.JobCount; i++)
        {
            JobExp[i] = saved.JobExp[i];
        }
    }

    public long GetScore()
    {
        long score = 0;
        score += Money / 100;
        score += (AtkLevel + 3) * (DefLevel + 3) * (HouseLevel + 3) * (Guards + Mobsters + 3 + Calc.TotalWeapons(this));
        return score;
    }

    public long GetCash()
    {
        return Math.Max(Money - Calc.GetBankLimit(this), 0L);
    }

    public long MoneyStolen { get; set; }
    public long MoneyLost { get; set; }
    public long GuardsKilled { get; set; }
    public long GuardsLost { get; set; }
    public long MobstersKilled { get; set; }
    public long MobstersLost { get; set; }
    public long WeaponsStolen { get; set; }
    public long WeaponsLost { get; set; }

    public FameLevel GetFameLevel()
    {
        return Enum.GetValues(typeof(FameLevel))
            .Cast<FameLevel>()
            .OrderByDescending(level => (int)level)
            .First(level => (long)level <= Fame);
    }
}