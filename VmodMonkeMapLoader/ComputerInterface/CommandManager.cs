using ComputerInterface;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class CommandManager : IInitializable
    {
        private readonly CommandHandler _commandHandler;
        
        public CommandManager(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Initialize()
        {
            _commandHandler.AddCommand(new Command(name: "whoami", argumentCount: 0, args =>
            {
                return "MONKE";
            }));
        }
    }
}