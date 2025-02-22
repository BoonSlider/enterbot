using Player;

namespace Bots;

public class OnlyEducation(int nameSuffix) : IBot(nameSuffix)
{
    public override string NamePrefix => "tarkpea";
    public override void PlayTurn(IPlayer p)
    {
        var d = p.MyData;
        var canGetEdu = Calculator.HowManyMovesCanSpendOnEdu(d);
        p.IncreaseEducation(canGetEdu).AssertOk();
        var maxJob = Calculator.GetMaxJobLevel(d);
        if (d.JobLevel != maxJob)
        {
            p.AcceptJob(maxJob).AssertOk();
        }
    }
}