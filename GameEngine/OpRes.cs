using Player;

namespace GameEngine;

public record OpRes(MessageType Type) : IOperationResult
{
    public required bool Success { get; init; }
    public Guid Id { get; } = Guid.NewGuid();
    public static IOperationResult Ok(MessageType messageType)
    {
        return new OpRes(messageType) { Success = true };
    }
    public static IOperationResult Err(MessageType messageType)
    {
        return new OpRes(messageType) { Success = false };
    }
}