using Player;

namespace Bots;

public class ClaudeBot(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "claudebot";
    private int _strategy = 0; // 0: build up, 1: offensive, 2: defensive
    private long _targetEdu = 18500; // Matches ArmorHoarder's initial education target
    private readonly Dictionary<Weapon, long> _weaponTargets = new()
    {
        { Weapon.Bat, 1000 },
        { Weapon.Knife, 2000 },
        { Weapon.Axe, 2000 },
        { Weapon.Armor, 50000 },
        { Weapon.Pistol, 5000 },
        { Weapon.Uzi, 100000 },
    };
    
    // Track when we last changed strategy to avoid oscillating
    private int _turnsSinceStrategyChange = 0;
    
    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        _turnsSinceStrategyChange++;
        
        // Evaluate and potentially update strategy every 5 turns
        if (_turnsSinceStrategyChange >= 5)
        {
            UpdateStrategy(p);
            _turnsSinceStrategyChange = 0;
        }
        
        var ops = new List<IOperationResult>();
        // Always prioritize education up to target
        if (d.Education < _targetEdu)
        {
            ops.AddRange(Common.AllMovesEducation(p, _targetEdu));
            if (d.Moves < 10) // If we spent most moves on education
            {
                ops.AddRange(OptimizeLevelsAndHouse(p));
                return ops;
            }
        }
        
        // Execute strategy-specific actions
        switch (_strategy)
        {
            case 0: // Build up
                ops.AddRange(ExecuteBuildUpStrategy(p));
                break;
            case 1: // Offensive
                ops.AddRange(ExecuteOffensiveStrategy(p));
                break;
            case 2: // Defensive
                ops.AddRange(ExecuteDefensiveStrategy(p));
                break;
        }
        
        // Always optimize levels and house
        ops.AddRange(OptimizeLevelsAndHouse(p));
        return ops;
    }
    
    private void UpdateStrategy(IPlayer p)
    {
        var d = p.MyData;
        var attackScore = Calc.PlayerSoloTotalAtk(d);
        var defenseScore = Calc.PlayerSoloTotalDef(d);
        var players = p.GetAllPlayers();
        
        // If we've reached our education target, evaluate what strategy to use
        if (d.Education >= _targetEdu)
        {
            // If we're significantly stronger on offense, lean into it
            if (attackScore > defenseScore * 1.5 && d.Mobsters >= 100)
            {
                _strategy = 1; // Offensive
                // Raise education target for late game
                _targetEdu = Calc.MaxEducation;
            }
            // If we're significantly stronger on defense, lean into it
            else if (defenseScore > attackScore * 1.5 && d.Guards >= 100)
            {
                _strategy = 2; // Defensive
                // Raise education target for late game
                _targetEdu = Calc.MaxEducation;
            }
            // Otherwise continue building up
            else
            {
                _strategy = 0; // Build up
            }
        }
        else
        {
            _strategy = 0; // Still building up if education target not met
        }
    }
    
    private IList<IOperationResult> ExecuteBuildUpStrategy(IPlayer p)
    {
        var d = p.MyData;
        
        var ops = new List<IOperationResult>();
        // Continue education beyond target if we have excess moves
        ops.AddRange(Common.AllMovesEducation(p));
        
        // Balanced approach to weapons
        ops.AddRange(AcquireBalancedWeapons(p));
        
        // Equal focus on mobsters and guards
        var movesSplit = d.Moves / 2;
        var mobsterMoves = Math.Min(movesSplit, d.Moves - Consts.AtkMoves); // Reserve some moves for attack
        
        // Hire mobsters with half the remaining moves
        if (mobsterMoves > 0)
        {
            var movesToUse = mobsterMoves;
            var mobsAllowed = movesToUse / Consts.MobsterMoves;
            ops.AddRange(Common.EnsureFood(p, mobsAllowed * Consts.MobsterFood));
            var canHire = Calc.CanHireMobsters(p.MyData);
            canHire = Math.Min(canHire, mobsAllowed);
            if (canHire > 0)
                ops.AddRange(p.HireMobsters(canHire));
        }
        
        // Use remaining moves for guards
        ops.AddRange(Common.AllMovesGuards(p, null, 0));
        
        // Attack if we have enough moves and mobsters
        if (Calc.CanAttack(p.MyData) && d.AtkLevel >= 20)
        {
            // Only attack if we feel confident
            if (d.Mobsters >= 50 && Calc.PlayerSoloTotalAtk(d) > 10000)
            {
                ops.AddRange(Common.AttackRandomPlayer(p));
            }
        }

        return ops;
    }
    
    private IList<IOperationResult> ExecuteOffensiveStrategy(IPlayer p)
    {
        var d = p.MyData;
        
        var ops = new List<IOperationResult>();
        // Focus primarily on attack weapons
        ops.AddRange();AcquireOffensiveWeapons(p);
        
        // Prioritize mobsters over guards
        var moveReserve = Math.Min(Consts.AtkMoves * 2, d.Moves / 3); // Reserve moves for attack
        ops.AddRange(Common.AllMovesMobsters(p, null, moveReserve));
        
        // Use leftover moves for education
        ops.AddRange(Common.AllMovesEducation(p));
        
        // Attack aggressively if we can
        if (Calc.CanAttack(p.MyData))
        {
            var players = p.GetAllPlayers();
            if (players.Count > 1)
            {
                // Try to attack multiple times if possible
                int attacks = Math.Min((int)(d.Moves / Consts.AtkMoves), 3);
                for (int i = 0; i < attacks; i++)
                {
                    if (Calc.CanAttack(p.MyData))
                    {
                        ops.AddRange(Common.AttackRandomPlayer(p));
                    }
                }
            }
        }

        return ops;
    }
    
    private IList<IOperationResult>  ExecuteDefensiveStrategy(IPlayer p)
    {
        var ops = new List<IOperationResult>();
        var d = p.MyData;
        
        // Focus primarily on defensive weapons
        ops.AddRange(AcquireDefensiveWeapons(p));
        
        // Prioritize guards over mobsters
        ops.AddRange(Common.AllMovesGuards(p, null, 0));
        
        // Use leftover moves for education and minimal offense
        var remainingMoves =  d.Moves;
        if (remainingMoves > Consts.AtkMoves * 2)
        {
            var mobsterMoves = Math.Min(remainingMoves / 4, d.Moves - Consts.AtkMoves);
            if (mobsterMoves > 0)
            {
                var movesToUse = mobsterMoves;
                var mobsAllowed = movesToUse / Consts.MobsterMoves;
                ops.AddRange(Common.EnsureFood(p, mobsAllowed * Consts.MobsterFood));
                var canHire = Calc.CanHireMobsters(p.MyData);
                canHire = Math.Min(canHire, mobsAllowed);
                if (canHire > 0)
                    ops.AddRange(p.HireMobsters(canHire));
            }
        }
        
        // Invest in education with remaining moves
        ops.AddRange(Common.AllMovesEducation(p));
        
        // Only attack if we have a strong position
        if (Calc.CanAttack(p.MyData) && d.AtkLevel >= 30 && d.Mobsters >= 100)
        {
            ops.AddRange(Common.AttackRandomPlayer(p));
        }

        return ops;
    }
    
    private IList<IOperationResult> AcquireBalancedWeapons(IPlayer p)
    {
        var d = p.MyData;
        var totalMoves = d.Moves;
        var movesForWeapons = totalMoves / 3; // Use up to 1/3 of moves for weapons
        
        if (movesForWeapons < Consts.BuyWeaponMoves * 2)
            return []; // Not enough moves to bother with weapons
            
        // Prioritize weapons based on cost-effectiveness and balance
        // First get basic weapons for both offense and defense
        var ops = new List<IOperationResult>();
        if (d.Weapons[Weapon.Bat] < _weaponTargets[Weapon.Bat])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Bat, _weaponTargets[Weapon.Bat], 0));
        }
        else if (d.Weapons[Weapon.Knife] < _weaponTargets[Weapon.Knife])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Knife, _weaponTargets[Weapon.Knife], 0));
        }
        else if (d.Weapons[Weapon.Armor] < _weaponTargets[Weapon.Armor])
        {
            // Armor is a major priority for balanced strategy
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, _weaponTargets[Weapon.Armor], 0));
        }
        else if (d.Weapons[Weapon.Uzi] < _weaponTargets[Weapon.Uzi])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, _weaponTargets[Weapon.Uzi], 0));
        }
        else if (d.Weapons[Weapon.Pistol] < _weaponTargets[Weapon.Pistol])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Pistol, _weaponTargets[Weapon.Pistol], 0));
        }
        else if (d.Weapons[Weapon.Axe] < _weaponTargets[Weapon.Axe])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Axe, _weaponTargets[Weapon.Axe], 0));
        }

        return ops;
    }
    
    private IList<IOperationResult> AcquireOffensiveWeapons(IPlayer p)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        // Prioritize weapons with high attack value
        if (d.Weapons[Weapon.Uzi] < _weaponTargets[Weapon.Uzi])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, _weaponTargets[Weapon.Uzi], 0));
        }
        else if (d.Weapons[Weapon.Pistol] < _weaponTargets[Weapon.Pistol])
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Pistol, _weaponTargets[Weapon.Pistol], 0));
        }
        else if (d.Weapons[Weapon.Knife] < _weaponTargets[Weapon.Knife] * 2) // Double knife target for offensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Knife, _weaponTargets[Weapon.Knife] * 2, 0));
        }
        else if (d.Weapons[Weapon.Axe] < _weaponTargets[Weapon.Axe] * 2) // Double axe target for offensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Axe, _weaponTargets[Weapon.Axe] * 2, 0));
        }
        else if (d.Weapons[Weapon.Armor] < _weaponTargets[Weapon.Armor] / 2) // Half armor target for offensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, _weaponTargets[Weapon.Armor] / 2, 0));
        }

        return ops;
    }
    
    private List<IOperationResult> AcquireDefensiveWeapons(IPlayer p)
    {
        var d = p.MyData;
        
        var ops = new List<IOperationResult>();
        // Prioritize weapons with high defense value
        if (d.Weapons[Weapon.Armor] < _weaponTargets[Weapon.Armor] * 2) // Double armor target for defensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, _weaponTargets[Weapon.Armor] * 2, 0));
        }
        else if (d.Weapons[Weapon.Axe] < _weaponTargets[Weapon.Axe] * 1.5) // More axes for defensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Axe, _weaponTargets[Weapon.Axe] * 3 / 2, 0));
        }
        else if (d.Weapons[Weapon.Bat] < _weaponTargets[Weapon.Bat] * 2) // Double bat target for defensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Bat, _weaponTargets[Weapon.Bat] * 2, 0));
        }
        else if (d.Weapons[Weapon.Uzi] < _weaponTargets[Weapon.Uzi] / 2) // Half Uzi target for defensive
        {
            ops.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, _weaponTargets[Weapon.Uzi] / 2, 0));
        }

        return ops;
    }
    
    private IList<IOperationResult> OptimizeLevelsAndHouse(IPlayer p)
    {
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.MaximizeAtkLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        return ops;
    }
}