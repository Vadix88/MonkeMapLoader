using ComputerInterface;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class CompManager : IInitializable
    {
        private readonly CommandHandler _commandHandler;

        // Request the CommandHandler
        // This gets resolved by zenject since we bind MyModCommandManager in the container
        public CompManager(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Initialize()
        {
            // Add a command
            _commandHandler.AddCommand(new Command(name: "whoami", argumentCount: 0, args =>
            {
                // args is an array of arguments (string) passed when entering the command
                // the command handler already checks if the correct amount of arguments is passed

                // the string you return is going to be shown in the terminal as a return message
                // you can break up the message into multiple lines by using \n
                return "MONKE";
            }));
        }
    }
}