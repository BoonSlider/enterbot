namespace Player;

public interface IPlayerActions
{
    public IOperationResult IncreaseEducation(long movesSpent);
    public IOperationResult AcceptJob(int jobLevel);
    public IOperationResult HireMobsters(long amount);
    public IOperationResult HireGuards(long amount);
    public IOperationResult UpdateAtkLevel(int desiredLevel);
    public IOperationResult UpdateDefLevel(int desiredLevel);
    public IOperationResult BuyFood(long foodAmount, Dictionary<MoonshineItem, long> moonshineItemCounts);
    public IOperationResult BuyHouse(long selectedHouseLevel);
}