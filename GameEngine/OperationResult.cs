using Player;

namespace GameEngine;

public record OperationResult : IOperationResult
{
    public required bool Success { get; init; }
    public string? Message { get; init; }
    public Guid Id { get; } = Guid.NewGuid();

    public static OperationResult Ok(string? message) => new() { Success = true, Message = message };
    public static OperationResult Error(string message) => new() { Success = false, Message = message };
    public static OperationResult MustBePositive => Error("Lubatud ainult positiivsed täisarvud.");
    public static OperationResult MustBeNonNegative => Error("Lubatud ainult mitte-negatiivsed täisarvud.");
    public static OperationResult NotEnoughMoves => Error("Pole piisavalt käike.");
    public static OperationResult NotEnoughMoney => Error("Pole piisavalt raha.");
    public static OperationResult NotEnoughFood => Error("Pole piisavalt toitu.");
    public static OperationResult NotEnoughEducation => Error("Pole piisavalt haridust.");
    public static OperationResult LevelAlreadyMaxed => Error("Level juba põhjas, pole kuhugi uuendada.");
    public static OperationResult LevelCantBeReduced => Error("Levelit ei saa vähendada.");
    public static OperationResult ChosenLevelAboveMax => Error("Nii suurt levelit pole olemas.");
    public static OperationResult NotEnoughFame => Error("Pole piisavalt kuulsust.");
    public static OperationResult AlreadyHave => Error("Juba olemas, ei saa osta.");
}