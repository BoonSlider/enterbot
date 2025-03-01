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
            return;
        Common.AllMovesEducation(p);
        var uzi = 200000;
        Common.AllMovesWeapon(p, Weapon.Uzi, uzi, 0);
        if (d.Weapons[Weapon.Uzi] == uzi)
        {
            await _innerDemon.PlayTurn(p);
        }

        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}