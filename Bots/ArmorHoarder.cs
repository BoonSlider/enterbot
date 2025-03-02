using Bots.Shared;
using Player;

namespace Bots;

public class ArmorHoarder(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "armor";
    private readonly AttackEveryone _innerDemon = new(-1);

    public override IList<IOperationResult> PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        var ops = new List<IOperationResult>();
        ops.AddRange(Common.AllMovesEducation(p, 18500));
        var armor = 100000;
        Common.AllMovesWeapon(p, Weapon.Armor, armor, 0);
        if (d.Weapons[Weapon.Armor] != armor)
            return ops;
        ops.AddRange(Common.AllMovesEducation(p));
        var uzi = 200000;
        ops.AddRange(Common.AllMovesWeapon(p, Weapon.Uzi, uzi, 0));
        if (d.Weapons[Weapon.Uzi] == uzi)
        {
            ops.AddRange(_innerDemon.PlayTurn(p));
        }

        ops.AddRange(Common.MaximizeAtkLvl(p));
        ops.AddRange(Common.MaximizeDefLvl(p));
        ops.AddRange(Common.MaximizeHouseLvl(p));
        return ops;
    }
}