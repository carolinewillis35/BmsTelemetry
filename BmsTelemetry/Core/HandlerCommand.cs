public enum HandlerCommandType
{
    Start,
    Stop,
    PollStep
}

public record HandlerCommand(
    HandlerCommandType Type,
    ClientCommand? ClientCmd = null
)
{
    public static HandlerCommand Start() => new(HandlerCommandType.Start);
    public static HandlerCommand Stop() => new(HandlerCommandType.Stop);
    public static HandlerCommand Poll(ClientCommand cmd) => new(HandlerCommandType.PollStep, cmd);
}
