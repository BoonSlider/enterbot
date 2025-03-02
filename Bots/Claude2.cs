using Bots.Shared;
using Player;

namespace Bots;

public class Claude2(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "claude2";
    private readonly Random _random = new();
    private int _phase;
    private int _turnCounter;
    private const long MinEducation = 8000; // Target for mid-tier jobs
    private const long EducationThreshold = 18500; // Good threshold for high-tier jobs
    private const long HighEducation = 510000; // For top job level (based on provided data)
    private const long WeaponThreshold = 500; // Basic weapon stockpile

    // Track attack history to learn about other players
    private readonly Dictionary<string, AttackHistory> _attackHistory = new();

    // Simple class to track attack history against other players
    private class AttackHistory
    {
        public int AttacksAttempted { get; set; }
        public int SuccessfulAttacks { get; set; }
        public long LastAttackStrength { get; set; }
        public double EstimatedDefenseStrength { get; set; }
    }

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        _turnCounter++;

        // Determine phase based on game state
        UpdatePhase(d);

        // Always try to get better jobs first for income
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.EnsureNotFeeding(p, d.TurnsPlayed/10, 10));
        ops.AddRange(OptimizeJob(p));

        // Act based on current phase
        switch (_phase)
        {
            case 0: // Early game - Focus on education and basic defense
                ops.AddRange(EarlyGameStrategy(p));
                break;
            case 1: // Mid-game - Build offensive capabilities
                ops.AddRange(MidGameStrategy(p));
                break;
            case 2: // Late game - Attack and expand
                ops.AddRange(LateGameStrategy(p));
                break;
        }

        // Always try to maximize house level if we have fame
        ops.AddRange(Common.MaximizeHouseLvl(p));

        // Make sure we have some food
        ops.AddRange(EnsureFoodSupply(p));
        return ops;
    }

    private void UpdatePhase(IPlayerData d)
    {
        // Use education and money as key indicators of game phase
        if (d.Education < MinEducation || d.Money < 100000)
        {
            _phase = 0; // Early game
        }
        else if (d.Education < EducationThreshold || d.Mobsters < 100 || d.AtkLevel < 10)
        {
            _phase = 1; // Mid-game
        }
        else
        {
            _phase = 2; // Late game
        }
    }

    private IList<IOperationResult> OptimizeJob(IPlayer p)
    {
        var d = p.MyData;
        var maxJob = Calc.GetMaxJobLevel(d);

        // If we can get a better job, take it
        if (d.JobLevel != maxJob)
        {
            return [p.AcceptJob(maxJob)];
        }

        return [];
    }

    private List<IOperationResult> EarlyGameStrategy(IPlayer p)
    {
        var d = p.MyData;

        // Early game is about maximizing education and income
        // Use up to 80% of moves for education
        long movesForEdu = (long)(d.Moves * 0.8);
        var ops = new List<IOperationResult>();
        if (movesForEdu > 0 && d.Education < EducationThreshold)
        {
            ops.AddRange(p.IncreaseEducation(movesForEdu));
        }

        // Upgrade attack and defense levels if we can afford it
        if (d.Money > 50000)
        {
            int targetAtkLvl = Math.Min(5, Calc.MaxAffordableAtkLvl(d));
            int targetDefLvl = Math.Min(5, Calc.MaxAffordableDefLvl(d));

            if (d.AtkLevel < targetAtkLvl)
                ops.AddRange(p.UpdateAtkLevel(targetAtkLvl));

            if (d.DefLevel < targetDefLvl)
                ops.AddRange(p.UpdateDefLevel(targetDefLvl));
        }

        // If we have spare money, buy some basic weapons
        ops.AddRange(BuyBasicWeapons(p));
        return ops;
    }

    private IList<IOperationResult> MidGameStrategy(IPlayer p)
    {
        var d = p.MyData;

        // Mid-game is about building attack and defense capabilities
        // Still invest in education if needed
        var ops = new List<IOperationResult>();
        if (d.Education < EducationThreshold)
        {
            long eduMoves = Math.Min(10, d.Moves / 2);
            if (eduMoves > 0)
            {
                ops.AddRange(p.IncreaseEducation(eduMoves));
            }
        }

        // Build up mobsters
        if (d is { Mobsters: < 100, Moves: > 20 })
        {
            ops.AddRange(Common.AllMovesMobsters(p, null, 20)); // Keep some moves for other actions
        }

        // Get some guards too
        if (d is { Guards: < 50, Money: > 100000, Moves: > 10 })
        {
            ops.AddRange(Common.AllMovesGuards(p, null, 0));
        }

        // Upgrade attack and defense
        if (d.Money > 200000)
        {
            int targetAtkLvl = Math.Min(15, Calc.MaxAffordableAtkLvl(d));
            int targetDefLvl = Math.Min(15, Calc.MaxAffordableDefLvl(d));

            if (d.DefLevel < targetDefLvl)
                ops.Add(p.UpdateDefLevel(targetDefLvl));

            if (d.AtkLevel < targetAtkLvl)
                ops.Add(p.UpdateAtkLevel(targetAtkLvl));
        }

        // Buy more weapons
        ops.AddRange(BuyStrategicWeapons(p));

        // Maybe attack if we have strong offense
        if (d is { Mobsters: >= Consts.MinimumMobstersToAttack, Moves: >= Consts.AtkMoves })
        {
            ops.AddRange(OpportunisticAttack(p));
        }

        return ops;
    }

    private IList<IOperationResult> LateGameStrategy(IPlayer p)
    {
        var d = p.MyData;

        // Late game is about attacking and maximizing score
        // Still improve education if we can afford top-tier jobs
        var ops = new List<IOperationResult>();
        if (d.Education < HighEducation && _turnCounter % 5 == 0 && d.Money > 1000000)
        {
            long eduMoves = Math.Min(5, d.Moves / 4);
            if (eduMoves > 0)
            {
                ops.AddRange(p.IncreaseEducation(eduMoves));
            }
        }

        // Maintain maximum attack and defense levels
        if (d is { AtkLevel: < Consts.MaxAtkDefLvl, Money: > 300000 })
        {
            ops.AddRange(Common.MaximizeAtkLvl(p));
        }

        if (d is { DefLevel: < Consts.MaxAtkDefLvl, Money: > 300000 })
        {
            ops.AddRange(Common.MaximizeDefLvl(p));
        }

        // Focus on mobsters for attacking
        if (d is { Moves: > 0, Money: > 500000 })
        {
            ops.AddRange(Common.AllMovesMobsters(p, null, 20)); // Keep some moves for attacks
        }

        // Buy high-end weapons
        ops.AddRange(BuyAdvancedWeapons(p));

        // Aggressively attack other players
        ops.AddRange(StrategicAttack(p));
        return ops;
    }

    private IList<IOperationResult> BuyBasicWeapons(IPlayer p)
    {
        var d = p.MyData;

        // In early game, focus on affordable weapons
        var ops = new List<IOperationResult>();
        if (d.Money > 20000 && d.Weapons[Weapon.Armor] < WeaponThreshold)
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, WeaponThreshold, 0));
        }
        else if (d.Money > 10000 && d.Weapons[Weapon.Bat] < WeaponThreshold)
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Bat, WeaponThreshold, 0));
        }

        return ops;
    }

    private IList<IOperationResult> BuyStrategicWeapons(IPlayer p)
    {
        var d = p.MyData;

        var ops = new List<IOperationResult>();
        // In mid-game, buy a mix of weapons
        if (d.Money > 50000)
        {
            // Prioritize armor for defense
            if (d.Weapons[Weapon.Armor] < 1000)
            {
                ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, 1000, 0));
            }
            // Then get some pistols
            else if (d.Weapons[Weapon.Pistol] < 500)
            {
                ops.AddRange(Common.AllMovesWeapon(p, Weapon.Pistol, 500, 0));
            }
            // And knives for cost efficiency
            else if (d.Weapons[Weapon.Knife] < 1000)
            {
                ops.AddRange(Common.AllMovesWeapon(p, Weapon.Knife, 1000, 0));
            }
        }

        return ops;
    }

    private IList<IOperationResult> BuyAdvancedWeapons(IPlayer p)
    {
        var d = p.MyData;

        // In late game, focus on high-end weapons
        var ops = new List<IOperationResult>();
        if (d.Money > 100000)
        {
            // Get Uzis for maximum attack power
            if (d.Weapons[Weapon.Uzi] < 2000)
            {
                ops.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, 2000, 0));
            }
            // Maintain armor stock
            else if (d.Weapons[Weapon.Armor] < 5000)
            {
                ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, 5000, 0));
            }
        }

        return ops;
    }

    private IList<IOperationResult> OpportunisticAttack(IPlayer p)
    {
        var d = p.MyData;

        // Only attack if we have enough mobsters and moves
        if (d is { Mobsters: >= Consts.MinimumMobstersToAttack, Moves: >= Consts.AtkMoves })
        {
            // Find a suitable target
            string target = FindTargetBasedOnHistory(p, attackThreshold: 0.6);

            var result = p.AttackPlayer(target, true);
            ProcessAttackResult(target, result);
            return [result];
        }

        return [];
    }

    private IList<IOperationResult> StrategicAttack(IPlayer p)
    {
        var d = p.MyData;

        // More aggressive attack strategy
        int attacksThisTurn = 0;
        int maxAttacksPerTurn = 3; // Limit attacks per turn to avoid depleting all mobsters

        var ops = new List<IOperationResult>();
        while (d is { Moves: >= Consts.AtkMoves, Mobsters: >= Consts.MinimumMobstersToAttack } &&
               attacksThisTurn < maxAttacksPerTurn)
        {
            // Higher threshold for more confident attacks
            string target = FindTargetBasedOnHistory(p, attackThreshold: 0.7);

            {
                var result = p.AttackPlayer(target, true);
                ProcessAttackResult(target, result);
                ops.Add(result);

                // If we lost a lot of mobsters, stop attacking
                if (result is IAttackResult { AttackSucceeded: false } attackResult &&
                    attackResult.MenLost > d.Mobsters / 3)
                {
                    break;
                }

                attacksThisTurn++;
            }
        }

        return ops;
    }

    private void ProcessAttackResult(string? targetId, IOperationResult result)
    {
        if (result is not IAttackResult attackResult) return;

        // Ensure we have an entry for this target
        if (targetId != null && !_attackHistory.ContainsKey(targetId))
        {
            _attackHistory[targetId] = new AttackHistory();
        }

        if (targetId != null)
        {
            var history = _attackHistory[targetId];
            history.AttacksAttempted++;

            if (attackResult.AttackSucceeded)
            {
                history.SuccessfulAttacks++;

                // We can estimate their defense was weaker than our attack
                history.EstimatedDefenseStrength = history.LastAttackStrength * 0.8; // Conservative estimate
            }
            else
            {
                // We can estimate their defense was stronger than our attack
                history.EstimatedDefenseStrength = history.LastAttackStrength * 1.5; // Conservative estimate
            }
        }
    }

    private string FindTargetBasedOnHistory(IPlayer p, double attackThreshold)
    {
        var d = p.MyData;
        var players = p.GetAllPlayers();
        var candidates = new List<(string playerId, double score)>();

        // Estimate our current attack strength
        long myAttackStrength = CalculateAttackStrength(d);

        foreach (var playerId in players)
        {
            if (playerId == p.Id) continue;

            var target = p.GetPlayerData(playerId);

            // Skip targets with no cash
            if (target.GetCash() < 10000) continue;

            double attackScore = EvaluateTargetForAttack(target, playerId, myAttackStrength);

            if (attackScore >= attackThreshold)
            {
                candidates.Add((playerId, attackScore));
            }
        }

        // Sort candidates by score (descending)
        candidates.Sort((a, b) => b.score.CompareTo(a.score));

        // Take the best candidate, if any
        if (candidates.Count > 0)
        {
            return candidates[0].playerId;
        }

        // If no good candidates but we need a target, pick a random one with cash
        if (candidates.Count == 0 && _random.Next(3) == 0) // 1 in 3 chance to pick random target
        {
            var cashTargets = new List<string?>();
            foreach (var playerId in players)
            {
                if (playerId == p.Id) continue;

                var target = p.GetPlayerData(playerId);
                if (target.GetCash() > 5000)
                {
                    cashTargets.Add(playerId);
                }
            }

            if (cashTargets.Count > 0)
            {
                return cashTargets[_random.Next(cashTargets.Count)] ?? string.Empty;
            }
        }

        return null!;
    }

    private double EvaluateTargetForAttack(IPlayerPublicData target, string? targetId, long myAttackStrength)
    {
        double baseScore = 0.5; // Default score
        double cashScore = 0;
        double historyScore = 0;

        // Factor 1: How much cash they have
        long targetCash = target.GetCash();
        if (targetCash > 1000000) cashScore = 0.30;
        else if (targetCash > 500000) cashScore = 0.25;
        else if (targetCash > 100000) cashScore = 0.20;
        else if (targetCash > 50000) cashScore = 0.15;
        else if (targetCash > 10000) cashScore = 0.10;

        // Factor 2: Previous attack history
        if (targetId != null && _attackHistory.TryGetValue(targetId, out var history))
        {
            // Successful attacks are a good sign
            if (history.SuccessfulAttacks > 0)
            {
                double successRate = (double)history.SuccessfulAttacks / history.AttacksAttempted;
                historyScore += successRate * 0.3;

                // If our attack strength is significantly higher than their estimated defense
                if (myAttackStrength > history.EstimatedDefenseStrength * 1.5)
                {
                    historyScore += 0.2;
                }
            }
            else if (history.AttacksAttempted > 0)
            {
                // Failed attacks reduce score
                historyScore -= 0.2;

                // Unless we're much stronger now
                if (myAttackStrength > history.LastAttackStrength * 2)
                {
                    historyScore += 0.1;
                }
            }
        }
        else
        {
            // Unknown targets get a small bonus (exploration)
            historyScore += 0.05;
        }

        // Consider house level as rough indicator of strength
        if (target.HouseLevel > 10) baseScore -= 0.1;
        else if (target.HouseLevel < 3) baseScore += 0.1;

        return baseScore + cashScore + historyScore;
    }

    private long CalculateAttackStrength(IPlayerData d)
    {
        // A rough estimation of our attack strength
        long baseStrength = d.Mobsters * 10;

        // Factor in attack level
        baseStrength *= (d.AtkLevel + 1);

        // Add strength from weapons (rough estimation)
        long weaponStrength = 0;
        weaponStrength += d.Weapons[Weapon.Bat] * 1;
        weaponStrength += d.Weapons[Weapon.Knife] * 2;
        weaponStrength += d.Weapons[Weapon.Axe] * 5;
        weaponStrength += d.Weapons[Weapon.Pistol] * 10;
        weaponStrength += d.Weapons[Weapon.Uzi] * 20;

        // Update attack history with our current strength
        foreach (var history in _attackHistory.Values)
        {
            history.LastAttackStrength = baseStrength + weaponStrength;
        }

        return baseStrength + weaponStrength;
    }

    private IList<IOperationResult> EnsureFoodSupply(IPlayer p)
    {
        var d = p.MyData;

        // Calculate how much food we need
        long neededFood = 100 + (d.Mobsters + d.Guards) * 2; // Base + some for our gang

        // Make sure we have enough food, especially if planning to hire more
        if (d.Food < neededFood && d.Money > neededFood * Consts.FoodPrice)
        {
            long foodToBuy = neededFood - d.Food;
            return[p.BuyFood(foodToBuy)];
        }

        return [];
    }
}