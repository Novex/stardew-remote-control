using StardewValley;
using System;
using System.Collections.Generic;

namespace RemoteControl.Commands
{
    abstract class Commands
    {
        private Dictionary<string, Action<Farmer, string>> commandList = new Dictionary<string, Action<Farmer, string>>();

        protected ModConfig Config;

        protected Commands(ModConfig config)
        {
            this.Config = config;
        }

        protected virtual bool shouldRun(Farmer fromPlayer)
        {
            return true;
        }

        protected virtual void fallbackCommand(Farmer farmer, string commandTrigger, string args)
        {

        }

        private void runCommand(Farmer farmer, string commandTrigger, string args)
        {
            Action<Farmer, string> command;

            if (commandList.TryGetValue(commandTrigger, out command))
            {
                command(farmer, args);
            } else
            {
                fallbackCommand(farmer, commandTrigger, args);
            }
        }

        public void ParseCommand(Farmer fromPlayer, string message)
        {
            if (message[0] != '!')
            {
                return;
            }

            string command = "";
            string args = "";

            try
            {
                command = message.Split(' ')[0].Substring(1);
                args = message.Substring(command.Length + 2);
            }
            catch { }

            if (shouldRun(fromPlayer)) {
                runCommand(fromPlayer, command, args);
            }

        }

        public void AddCommand(string commandString, Action<Farmer, string> fn)
        {
            commandList.Add(commandString, fn);
        }

    }
}
