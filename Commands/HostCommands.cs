using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

using static RemoteControl.Utilities;

namespace RemoteControl.Commands
{
    // Commands that can only be run by the host
    class HostCommands : Commands
    {
        private bool shouldQuit = false;

        public HostCommands(ModConfig config) : base(config)
        {
            AddCommand("shutdown", shutdown);
        }

        protected override bool shouldRun(Farmer fromPlayer)
        {
            return Game1.player == fromPlayer;
        }

        private void shutdown(Farmer fromPlayer, string args)
        {
            publishSystemMessage("Shutting down!");

            if (Game1.dayOfMonth > 1)
            {
                // Move back a day if we can
                Game1.stats.DaysPlayed--;
                Game1.dayOfMonth--;
            }

            // Kick everyone out
            foreach (Farmer farmer in (IEnumerable<Farmer>)Game1.otherFarmers.Values.ToList())
            {
                Game1.server.kick(farmer.UniqueMultiplayerID);
            }

            // Put host player to sleep
            Game1.player.isInBed.Value = true;
            Game1.currentLocation.answerDialogueAction("Sleep_Yes", (string[])null);

            // And quit after save
            shouldQuit = true;
        }

        public void OnSaved(object sender, SavedEventArgs e)
        {
            if (!shouldQuit)
            {
                return;
            }

            // Set the quit flag, the game should pick this up
            Game1.quit = true;
        }

    }
}
