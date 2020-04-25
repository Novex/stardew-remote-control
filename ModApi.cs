using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControl
{
    /* Allow other mods to add custom commands
     * eg. commandTrigger = dosomething would run functionToRun when someone messages !dosomething
     */
    public class ModApi
    {
        private Action<string, Action<Farmer, string>> addHostCommand;
        private Action<string, Action<Farmer, string>> addAdminCommand;
        private Action<string, Action<Farmer, string>> addUserCommand;

        public ModApi(Action<string, Action<Farmer, string>> addHostCommand, Action<string, Action<Farmer, string>> addAdminCommand, Action<string, Action<Farmer, string>> addUserCommand)
        {
            this.addHostCommand = addHostCommand;
            this.addAdminCommand = addAdminCommand;
            this.addUserCommand = addUserCommand;
        }

        public void AddHostCommand(string commandTrigger, Action<Farmer, string> functionToRun)
        {
            addHostCommand(commandTrigger, functionToRun);
        }

        public void AddAdminCommand(string commandTrigger, Action<Farmer, string> functionToRun)
        {
            addAdminCommand(commandTrigger, functionToRun);
        }

        public void AddUserCommand(string commandTrigger, Action<Farmer, string> functionToRun)
        {
            addUserCommand(commandTrigger, functionToRun);
        }

        // Send a message to everyone
        public void publishSystemMessage(string msg)
        {
            Utilities.publishSystemMessage(msg);
        }

        // Send a PM
        public void publishPrivateMessage(Farmer toPlayer, string msg)
        {
            Utilities.publishPrivateMessage(toPlayer, msg);
        }
    }
}
