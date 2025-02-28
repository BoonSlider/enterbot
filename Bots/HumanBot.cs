using Player;

namespace Bots;

public class HumanBot(AutomationSettings a) : IBot(-1)
{
    public override string NamePrefix => "human";

    public override void PlayTurn(IPlayer p)
    {
        if (!a.IsActive) return;
        if (a.EducationLimit is {} edu)
            Common.AllMovesEducation(p, edu);
    }
}