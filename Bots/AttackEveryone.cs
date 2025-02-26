using Player;

namespace Bots;

public class AttackEveryone(int nameSuffix) : IBot(nameSuffix)
{
    private Random _random = new Random();
    public override string NamePrefix => "sarimõrvar";
    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        if (d.Moves < 100)
        {
            return;
        }

        Common.EnsureFood(p, 10000);
        var menHire = (d.Moves - Constants.AtkMoves)/Constants.MobsterMoves;
        var canHire = Math.Min(Calculator.CanHireMobsters(d), menHire);
        p.HireMobsters(canHire);
        if (Calculator.CanAttack(d))
        {
            var players = p.GetAllPlayers();
            var target = _random.Next(players.Count);
            while (players[target] == p.Id)
            {
                target = _random.Next(players.Count);
            }
            p.AttackPlayer(players[target], false);
        }
        Common.MaximizeAtkLvl(p, d);
        Common.MaximizeDefLvl(p, d);
        Common.MaximizeHouseLvl(p, d);
    }
}