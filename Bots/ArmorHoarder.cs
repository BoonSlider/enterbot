using Player;

namespace Bots;

public class ArmorHoarder(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "armor";
    private readonly AttackEveryone _innerDemon = new(-1);
    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        Common.AllMovesEducation(p, 18500);
        Common.AllMovesWeapon(p, Weapon.Armor, 100000);
        Common.AllMovesEducation(p);
        var uzi = 200000;
        Common.AllMovesWeapon(p, Weapon.Uzi, uzi);
        if (d.Weapons[Weapon.Uzi] == uzi)
        {
            _innerDemon.PlayTurn(p);
        }
        Common.MaximizeAtkLvl(p);
        Common.MaximizeDefLvl(p);
        Common.MaximizeHouseLvl(p);
    }
}