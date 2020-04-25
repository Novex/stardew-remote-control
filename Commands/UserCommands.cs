using StardewValley;

using static RemoteControl.Utilities;

namespace RemoteControl.Commands
{
    // Commands that can be run by everyone
    class UserCommands : Commands
    {
        public UserCommands(ModConfig config) : base(config)
        {
            AddCommand("kitz", test);
        }

        private void test(Farmer fromPlayer, string args)
        {
            publishSystemMessage("kitz.. esmllz :O");
        }
    }
}
