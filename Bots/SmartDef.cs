using Bots.Shared;
using Player;
using Player.House;

namespace Bots;

public class SmartDef(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "gaara";
    private AtkStats? _atkStats;
    private const long FearDuration = 96 * 30;

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        EnsureInitialized(p);
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.EnsureNotFeeding(p, d.TurnsPlayed/10, 10));
        ops.AddRange(Common.AllMovesProtectedGuards(p, 0));
        var weaponLimit = Calc.GetMaxGuardedWeapons(d);
        foreach (var w in Enum.GetValues<Weapon>())
        {
            ops.AddRange(Common.AllMovesWeapon(p, w, weaponLimit, 0));
        }

        if (d.Guards == Calc.CurrentHouse(d).ProtectedGuards && d.HouseLevel != Consts.MaxHouseLvl)
        {
            ops.AddRange(Common.AllMovesMobsters(p, Consts.MinimumMobstersToAttack, 0));
            while (Calc.CanAttack(d))
            {
                var possibleTargets = _atkStats!.HaventFailedAgainstSince(d.TurnsPlayed - FearDuration)
                    .Select(p.GetPlayerData).ToList();
                var target = ChooseBestTarget(possibleTargets);
                var res = Common.SafeAttackPlayer(p, target, false);
                _atkStats.SaveAtkResult(res!);
                ops.Add(res!);
            }
        }

        if (d.Guards == Houses.MaxHouse.ProtectedGuards)
            ops.AddRange(Common.AllMovesMobsters(p, null, 100));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        var nextHouse = Calc.GetNextHouse(d);
        if (d.Mobsters > 10000 || nextHouse != null && nextHouse.Price <= d.Money)
        {
            ops.AddRange(Common.MaximizeAtkLvl(p));
        }

        ops.AddRange(Common.AllMovesEducation(p, null, 100));

        return ops;
    }

    private void EnsureInitialized(IPlayer p)
    {
        _atkStats ??= new AtkStats(p.Id, p.GetAllPlayers());
    }

    private string ChooseBestTarget(List<IPlayerPublicData> targets)
    {
        return targets
            // OrderByDescending(t => _atkStats!.LastAtkWeapons(t.Id))
            // .ThenByDescending(t => _atkStats!.GetLastFameChange(t.Id) ?? 1)
            // .ThenByDescending(t => t.GetFameLevel())
            .OrderByDescending(t => t.GetCash())
            .First().Id;
    }
}