namespace Player;

public interface IOperationResult
{
    public bool Success { get; }
    public string? Message { get; }
    public Guid Id { get; }

    public void AssertOk()
    {
        if (!Success)
        {
            throw new Exception("Result was not successful.");
        }
    }
}