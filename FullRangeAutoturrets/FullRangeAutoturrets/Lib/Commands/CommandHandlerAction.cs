namespace FullRangeAutoturrets.Lib.Commands
{
    /// <summary>
    /// To enable us to send along a player object, a custom event action is implemented
    /// </summary>
    public delegate void CommandHandlerAction(object sender, object[] args);
}