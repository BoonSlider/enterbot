using Player;
using System.Collections.Generic;

namespace Bots;

public class BalancedMafiaBoss(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "claude2";
    private readonly Random _random = new();
    private int _phase = 0;
    private int _turnCounter = 0;
    private const long MIN_EDUCATION = 8000; // Target for mid-tier jobs
    private const long EDUCATION_THRESHOLD = 18500; // Good threshold for high-tier jobs
    private const long HIGH_EDUCATION = 510000; // For top job level (based on provided data)
    private const long DEFENSE_BUFFER = 30000; // Money to keep for defense upgrades
    private const long WEAPON_THRESHOLD = 500; // Basic weapon stockpile
    
    // Track attack history to learn about other players
    private Dictionary<string, AttackHistory> _attackHistory = new();

    // Simple class to track attack history against other players
    private class AttackHistory
    {
        public int AttacksAttempted { get; set; } = 0;
        public int SuccessfulAttacks { get; set; } = 0;
        public long LastAttackTurn { get; set; } = 0;
        public long LastMoneyStolenAmount { get; set; } = 0;
        public long LastAttackStrength { get; set; } = 0;
        public bool LastAttackSucceeded { get; set; } = false;
        public long LastMenLost { get; set; } = 0;
        public double EstimatedDefenseStrength { get; set; } = 0;
    }

    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        _turnCounter++;
        
        // Determine phase based on game state
        UpdatePhase(d);
        
        // Always try to get better jobs first for income
        OptimizeJob(p);
        
        // Act based on current phase
        switch (_phase)
        {
            case 0: // Early game - Focus on education and basic defense
                EarlyGameStrategy(p);
                break;
            case 1: // Mid game - Build offensive capabilities
                MidGameStrategy(p);
                break;
            case 2: // Late game - Attack and expand
                LateGameStrategy(p);
                break;
        }
        
        // Always try to maximize house level if we have fame
        Common.MaximizeHouseLvl(p);
        
        // Make sure we have some food
        EnsureFoodSupply(p);
    }

    private void UpdatePhase(IPlayerData d)
    {
        // Use education and money as key indicators of game phase
        if (d.Education < MIN_EDUCATION || d.Money < 100000)
        {
            _phase = 0; // Early game
        }
        else if (d.Education < EDUCATION_THRESHOLD || d.Mobsters < 100 || d.AtkLevel < 10)
        {
            _phase = 1; // Mid game
        }
        else
        {
            _phase = 2; // Late game
        }
    }

    private void OptimizeJob(IPlayer p)
    {
        var d = p.MyData;
        var maxJob = Calc.GetMaxJobLevel(d);
        
        // If we can get a better job, take it
        if (d.JobLevel != maxJob)
        {
            p.AcceptJob(maxJob);
        }
    }

    private void EarlyGameStrategy(IPlayer p)
    {
        var d = p.MyData;
        
        // Early game is about maximizing education and income
        // Use up to 80% of moves for education
        long movesForEdu = (long)(d.Moves * 0.8);
        if (movesForEdu > 0 && d.Education < EDUCATION_THRESHOLD)
        {
            p.IncreaseEducation(movesForEdu);
        }
        
        // Upgrade attack and defense levels if we can afford it
        if (d.Money > 50000)
        {
            int targetAtkLvl = Math.Min(5, Calc.MaxAffordableAtkLvl(d));
            int targetDefLvl = Math.Min(5, Calc.MaxAffordableDefLvl(d));
            
            if (d.AtkLevel < targetAtkLvl)
                p.UpdateAtkLevel(targetAtkLvl);
                
            if (d.DefLevel < targetDefLvl)
                p.UpdateDefLevel(targetDefLvl);
        }
        
        // If we have spare money, buy some basic weapons
        BuyBasicWeapons(p);
    }

    private void MidGameStrategy(IPlayer p)
    {
        var d = p.MyData;
        
        // Mid game is about building attack and defense capabilities
        // Still invest in education if needed
        if (d.Education < EDUCATION_THRESHOLD)
        {
            long eduMoves = Math.Min(10, d.Moves / 2);
            if (eduMoves > 0)
            {
                p.IncreaseEducation(eduMoves);
            }
        }
        
        // Build up mobsters
        if (d.Mobsters < 100 && d.Moves > 20)
        {
            Common.AllMovesMobsters(p, null, 20); // Keep some moves for other actions
        }
        
        // Get some guards too
        if (d.Guards < 50 && d.Money > 100000 && d.Moves > 10)
        {
            Common.AllMovesGuards(p, null, 0);
        }
        
        // Upgrade attack and defense
        if (d.Money > 200000)
        {
            int targetAtkLvl = Math.Min(15, Calc.MaxAffordableAtkLvl(d));
            int targetDefLvl = Math.Min(15, Calc.MaxAffordableDefLvl(d));
            
            if (d.DefLevel < targetDefLvl)
                p.UpdateDefLevel(targetDefLvl);
                
            if (d.AtkLevel < targetAtkLvl)
                p.UpdateAtkLevel(targetAtkLvl);
        }
        
        // Buy more weapons
        BuyStrategicWeapons(p);
        
        // Maybe attack if we have strong offense
        if (d.Mobsters >= Consts.MinimumMobstersToAttack && d.Moves >= Consts.AtkMoves)
        {
            OpportunisticAttack(p);
        }
    }

    private void LateGameStrategy(IPlayer p)
    {
        var d = p.MyData;
        
        // Late game is about attacking and maximizing score
        // Still improve education if we can afford top-tier jobs
        if (d.Education < HIGH_EDUCATION && _turnCounter % 5 == 0 && d.Money > 1000000)
        {
            long eduMoves = Math.Min(5, d.Moves / 4);
            if (eduMoves > 0)
            {
                p.IncreaseEducation(eduMoves);
            }
        }
        
        // Maintain maximum attack and defense levels
        if (d.AtkLevel < Consts.MaxAtkDefLvl && d.Money > 300000)
        {
            Common.MaximizeAtkLvl(p);
        }
        
        if (d.DefLevel < Consts.MaxAtkDefLvl && d.Money > 300000)
        {
            Common.MaximizeDefLvl(p);
        }
        
        // Focus on mobsters for attacking
        if (d.Moves > 0 && d.Money > 500000)
        {
            Common.AllMovesMobsters(p, null, 20); // Keep some moves for attacks
        }
        
        // Buy high-end weapons
        BuyAdvancedWeapons(p);
        
        // Aggressively attack other players
        StrategicAttack(p);
    }

    private void BuyBasicWeapons(IPlayer p)
    {
        var d = p.MyData;
        
        // In early game, focus on affordable weapons
        if (d.Money > 20000 && d.Weapons[Weapon.Armor] < WEAPON_THRESHOLD)
        {
            Common.AllMovesWeapon(p, Weapon.Armor, WEAPON_THRESHOLD, 0);
        }
        else if (d.Money > 10000 && d.Weapons[Weapon.Bat] < WEAPON_THRESHOLD)
        {
            Common.AllMovesWeapon(p, Weapon.Bat, WEAPON_THRESHOLD, 0);
        }
    }

    private void BuyStrategicWeapons(IPlayer p)
    {
        var d = p.MyData;
        
        // In mid game, buy a mix of weapons
        if (d.Money > 50000)
        {
            // Prioritize armor for defense
            if (d.Weapons[Weapon.Armor] < 1000)
            {
                Common.AllMovesWeapon(p, Weapon.Armor, 1000, 0);
            }
            // Then get some pistols
            else if (d.Weapons[Weapon.Pistol] < 500)
            {
                Common.AllMovesWeapon(p, Weapon.Pistol, 500, 0);
            }
            // And knives for cost efficiency
            else if (d.Weapons[Weapon.Knife] < 1000)
            {
                Common.AllMovesWeapon(p, Weapon.Knife, 1000, 0);
            }
        }
    }

    private void BuyAdvancedWeapons(IPlayer p)
    {
        var d = p.MyData;
        
        // In late game, focus on high-end weapons
        if (d.Money > 100000)
        {
            // Get Uzis for maximum attack power
            if (d.Weapons[Weapon.Uzi] < 2000)
            {
                Common.AllMovesWeapon(p, Weapon.Uzi, 2000, 0);
            }
            // Maintain armor stock
            else if (d.Weapons[Weapon.Armor] < 5000)
            {
                Common.AllMovesWeapon(p, Weapon.Armor, 5000, 0);
            }
        }
    }

    private void OpportunisticAttack(IPlayer p)
    {
        var d = p.MyData;
        
        // Only attack if we have enough mobsters and moves
        if (d.Mobsters >= Consts.MinimumMobstersToAttack && d.Moves >= Consts.AtkMoves)
        {
            // Find a suitable target
            string target = FindTargetBasedOnHistory(p, attackThreshold: 0.6);
            
            if (target != null)
            {
                var result = p.AttackPlayer(target, true);
                ProcessAttackResult(target, result, d.TurnsPlayed);
            }
        }
    }

    private void StrategicAttack(IPlayer p)
    {
        var d = p.MyData;
        
        // More aggressive attack strategy
        int attacksThisTurn = 0;
        int maxAttacksPerTurn = 3; // Limit attacks per turn to avoid depleting all mobsters
        
        while (d.Moves >= Consts.AtkMoves && d.Mobsters >= Consts.MinimumMobstersToAttack && attacksThisTurn < maxAttacksPerTurn)
        {
            // Higher threshold for more confident attacks
            string target = FindTargetBasedOnHistory(p, attackThreshold: 0.7);
            
            if (target != null)
            {
                var result = p.AttackPlayer(target, true);
                ProcessAttackResult(target, result, d.TurnsPlayed);
                
                // If we lost a lot of mobsters, stop attacking
                if (result is IAttackResult attackResult && !attackResult.AttackSucceeded && attackResult.MenLost > d.Mobsters / 3)
                {
                    break;
                }
                
                attacksThisTurn++;
            }
            else
            {
                // No good targets found
                break;
            }
        }
    }
    
    private void ProcessAttackResult(string targetId, IOperationResult result, long currentTurn)
    {
        if (result is not IAttackResult attackResult) return;
        
        // Ensure we have an entry for this target
        if (!_attackHistory.ContainsKey(targetId))
        {
            _attackHistory[targetId] = new AttackHistory();
        }
        
        var history = _attackHistory[targetId];
        history.AttacksAttempted++;
        history.LastAttackTurn = currentTurn;
        
        if (attackResult.AttackSucceeded)
        {
            history.SuccessfulAttacks++;
            history.LastAttackSucceeded = true;
            history.LastMoneyStolenAmount = attackResult.MoneyStolen;
            
            // We can estimate their defense was weaker than our attack
            history.EstimatedDefenseStrength = history.LastAttackStrength * 0.8; // Conservative estimate
        }
        else
        {
            history.LastAttackSucceeded = false;
            history.LastMenLost = attackResult.MenLost;
            
            // We can estimate their defense was stronger than our attack
            history.EstimatedDefenseStrength = history.LastAttackStrength * 1.5; // Conservative estimate
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
            var cashTargets = new List<string>();
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
                return cashTargets[_random.Next(cashTargets.Count)];
            }
        }
        
        return null;
    }
    
    private double EvaluateTargetForAttack(IPlayerPublicData target, string targetId, long myAttackStrength)
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
        if (_attackHistory.TryGetValue(targetId, out var history))
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

    private void EnsureFoodSupply(IPlayer p)
    {
        var d = p.MyData;
        
        // Calculate how much food we need
        long neededFood = 100 + (d.Mobsters + d.Guards) * 2; // Base + some for our gang
        
        // Make sure we have enough food, especially if planning to hire more
        if (d.Food < neededFood && d.Money > neededFood * Consts.FoodPrice)
        {
            long foodToBuy = neededFood - d.Food;
            p.BuyFood(foodToBuy);
        }
    }
}