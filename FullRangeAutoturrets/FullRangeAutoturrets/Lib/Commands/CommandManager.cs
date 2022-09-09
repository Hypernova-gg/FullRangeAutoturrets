using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FullRangeAutoturrets.Lib.Logging;

namespace FullRangeAutoturrets.Lib.Commands
{
    public class CommandManager
    {
        
        /// <summary>
        /// List of all registered commands
        /// </summary>
        private List<Command> RegisteredCommands { get; set; }
        
        /// <summary>
        /// Constructor which resets all saved commands
        /// </summary>
        public CommandManager()
        {
            this.Reset();
        }
        
        /// <summary>
        /// Initialize the command manager
        /// </summary>
        public void Reset()
        {
            this.RegisteredCommands = new List<Command>();
        }
        
        /// <summary>
        /// Registers a command by string, parses the command and adds it to the list of registered commands
        /// </summary>
        /// <param name="commandName">String used as command identifier, invokable through a console or chat command</param>
        /// <param name="action">List of (a) invokable method(s) to execute on command</param>
        /// <param name="type">Whodunit?</param>
        /// <param name="flags">Flags passed to customize command execution or invokation rules</param>
        private void RegisterCommand(string commandName, CommandHandlerAction action, CommandType type, CommandFlag flags)
        {
            commandName = commandName.ToLower();
            
            var command = RegisteredCommands.Find(x => x.Name == commandName && x.Type == type);
            if (command != null)
            {
                command.AddListener(action);
                return;
            }
            
            // This is generally good practise, we don't want to accidentally overwrite a command that's crucial to game functions
            if (flags.HasFlag(CommandFlag.IncludePrefix))
                commandName = Main.ModShortName.ToLower() + "." + commandName;
            
            
            string stringType = type == CommandType.Console ? "Console" : (type == CommandType.Chat ? "Chat" : "RCON" );
            LoggingManager.Log("Registering " + stringType + " command " + commandName);
            
            Command cmd = new Command(commandName, type, action, flags);
            RegisteredCommands.Add(cmd);
        }
    
        public void RegisterRCON(string commandName, CommandHandlerAction action, CommandFlag flags = CommandFlag.None) =>  this.RegisterCommand(commandName, action, CommandType.RCON, flags);
        public void RegisterChat(string commandName, CommandHandlerAction action, CommandFlag flags = CommandFlag.None) =>  this.RegisterCommand(commandName, action, CommandType.Chat, flags);
        public void RegisterConsole(string commandName, CommandHandlerAction action, CommandFlag flags = CommandFlag.None) =>  this.RegisterCommand(commandName, action, CommandType.Console, flags);

        /// <summary>
        /// Make sure the command is properly split and filtered
        /// </summary>
        /// <param name="commandString">Raw command string</param>
        /// <returns>KeyValuePair where the key is the command name and value is a collection of args</returns>
        private KeyValuePair<string, string[]> ParseCommand(string commandString)
        {
            string[] commandParts = commandString.Split(' ');
            string commandName = commandParts[0].ToLower();
            commandName = Regex.Replace(commandName, "[^a-zA-Z0-9/_,.!]", "");
            commandName = Regex.Replace(commandName, @"\r\n?|\n", "");
            
            string[] args = commandParts.Skip(1).ToArray();
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Regex.Replace(args[i], "[^a-zA-Z0-9/_,.!]", "");
            }

            return new KeyValuePair<string, string[]>(commandName, args);
        }

        /// <summary>
        /// Entry-point for all commands, this method will parse the command and execute it if it's registered
        /// </summary>
        /// <param name="options">Will contain info such as the connection of the executing player, or properties to indicate RCON, etc.</param>
        /// <param name="strCommand">Raw, unparsed command string</param>
        /// <param name="args">Basically useless</param>
        /// <returns>A bool to indicate whether or not the original code should execute after command execution</returns>
        public bool Handler(ConsoleSystem.Option options, string strCommand, params object[] args)
        {
            // sanitize the strCommand string allowing only letters, numbers and some special characters
            KeyValuePair<string, string[]> parsedCommand = ParseCommand(strCommand);
            string commandName = parsedCommand.Key;
            object[] commandArgs = parsedCommand.Value as object[];
            
            CommandType cmdType = CommandType.RCON;
            if (options.Connection != null)
            {
                if (options.Connection != null && commandName == "chat.say" && commandArgs.Length > 0)
                {
                    bool isCommand = commandArgs[0].ToString().StartsWith("/") || commandArgs[0].ToString().StartsWith("!");
                    if (!isCommand)
                    {
                        // Its just a chat message, not a command
                        return true;
                    }
                    
                    commandName = commandArgs[0].ToString().Substring(1);
                    commandArgs = commandArgs.Skip(1).ToArray();
                    cmdType = CommandType.Chat;
                }
                else
                {
                    cmdType = CommandType.Console;
                }
            }

            try
            {
                Command command =
                    RegisteredCommands.FirstOrDefault(x => x.Name == commandName && x.Type == cmdType) as Command;
                
                if (command == null || command.HasListeners() == false)
                    return true; // No command found, return true to continue original code execution
                
                BasePlayer sender = options.Connection?.player as BasePlayer ?? null;

                command.Execute(sender, commandArgs);

                return !command.Flags.HasFlag(CommandFlag.BlockExecution); // Return false if we want to block the original code execution
                
            } catch (Exception e)
            {
                LoggingManager.Log("Error while executing command " + commandName + ": " + e.Message);
                return true;
            }
        }

    }
}