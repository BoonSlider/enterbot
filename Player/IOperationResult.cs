namespace Player;

public interface IOperationResult
{
    public bool Success { get; }
    public MessageType Type { get; }
    public Guid Id { get; }

    public void AssertOk()
    {
        if (!Success)
        {
            throw new Exception("Result was not successful.");
        }
    }
}