using System;
using System.Security.Cryptography;

namespace FullRangeAutoturrets.Lib.Commands
{
    /// <summary>
    /// Type of command to signify the type of command to be executed.
    /// </summary>
    public enum CommandType
    {
        RCON = 0,
        Console = 1,
        Chat = 2,
    }
    
    /// <summary>
    /// Flags to add to commands to control how they are executed.
    /// </summary>
    [Flags]
    public enum CommandFlag
    {
        None = 0,
        Admin = 1,
        BlockExecution = 2,
        IncludePrefix = 3,
    }

    public class Command
    {
        public string Name;
        public CommandType Type;
        public CommandFlag Flags;
        
        /// <summary>
        /// Custom event to be executed when the command is called, containing the sender (optional Player) and args
        /// </summary>
        private event CommandHandlerAction _action;
        
        /// <summary>
        /// Constructor for a command
        /// </summary>
        /// <param name="name">The command itself, being a short string like "reload" or something.</param>
        /// <param name="type">Whodunit?</param>
        /// <param name="action">List of (a) invokable method(s) to execute on command</param>
        /// <param name="flags">Flags passed to customize command execution or invokation rules</param>
        public Command(string name, CommandType type, CommandHandlerAction action, CommandFlag flags)
        {
            Name = name;
            Type = type;
            Flags = flags;
            
            this.AddListener(action);
        }
        
        /// <summary>
        /// Method to check if the command has any methods to invoke at execution.
        /// </summary>
        /// <returns>Bool to indicate the existance of invokable methods</returns>
        public bool HasListeners()
        {
            return _action != null;
        }
        
        /// <summary>
        /// Push a method to the command to be invoked at execution.
        /// </summary>
        /// <param name="action">Method to execute</param>
        public void AddListener(CommandHandlerAction action)
        {
            _action += action;
        }
        
        /// <summary>
        /// Remove a method from the command to be invoked at execution.
        /// </summary>
        /// <param name="action">Method to remove</param>
        public void RemoveListener(CommandHandlerAction action)
        {
            _action -= action;
        }
        
        /// <summary>
        /// Invoke the command, executing all methods attached to it.
        /// </summary>
        /// <param name="sender">Player object or null</param>
        /// <param name="args">Argument list to pass along to executed method</param>
        public void Execute(object sender, params object[] args)
        {
            _action?.Invoke(sender, args);
        }
    }
}