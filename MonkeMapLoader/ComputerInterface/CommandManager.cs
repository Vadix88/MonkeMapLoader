using ComputerInterface;
using System;
using System.Collections.Generic;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class CommandManager : IInitializable, IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private List<CommandToken> _commandTokens;

        public CommandManager(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Initialize()
        {
        }

        public void RegisterCommand(Command cmd)
        {
            var token = _commandHandler.AddCommand(cmd);
            _commandTokens.Add(token);
        }

        public void UnregisterAllCommands()
        {
            foreach (var token in _commandTokens)
            {
                token.UnregisterCommand();
            }
        }

        public void Dispose()
        {
            UnregisterAllCommands();
        }
    }
}