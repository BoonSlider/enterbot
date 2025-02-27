using Bots;

namespace GameEngine;

public class BotWithData
{
    public required Player Player { get; init; }
    public required IBot Strategy { get; init; }
}