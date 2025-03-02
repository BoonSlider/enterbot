using Bots.Shared;
using Player;
using Player.House;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "sheldon";

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.AllMovesEducation(p, null, 0));
        ops.AddRange(Common.AllMovesGuards(p, Houses.MaxHouse.ProtectedGuards, 0));
        ops.AddRange(Common.AllMovesWeapon(p, Weapon.Armor, Houses.MaxHouse.ProtectedGuards*Consts.WeaponGuardedRate, 0));
        ops.AddRange(Common.AllMovesGuards(p, null, 0));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        if (d is { DefLevel: Consts.MaxAtkDefLvl })
        {
            ops.AddRange(Common.MaximizeAtkLvl(p));
        }

        return ops;
    }
}