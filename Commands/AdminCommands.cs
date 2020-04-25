using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using static RemoteControl.Utilities;

namespace RemoteControl.Commands
{
    // Commands that can be run only by players with Admin
    class AdminCommands : Commands
    {
        public AdminCommands(ModConfig config) : base(config)
        {
            AddCommand("admin", addAdmin);
            AddCommand("unadmin", removeAdmin);

            AddCommand("listadmin", listAdmin);
            AddCommand("adminlist", listAdmin);

            AddCommand("listplayers", listPlayers);
            AddCommand("playerlist", listPlayers);

            AddCommand("buildcabin", buildCabin);
            AddCommand("cabin", buildCabin);
            AddCommand("newcabin", buildCabin);
        }

        protected override bool shouldRun(Farmer fromPlayer)
        {
            return Config.isAdmin(fromPlayer);
        }
        protected override void fallbackCommand(Farmer farmer, string commandTrigger, string args)
        {
            Game1.chatBox.textBoxEnter($"/{commandTrigger} {args}");
        }

        // --- Command Implementations ---
        private void addAdmin(Farmer fromPlayer, string args)
        {
            Config.addAdmin(FirstFarmerByName(args));
            publishSystemMessage($"{args} added to admin list");
        }

        private void removeAdmin(Farmer fromPlayer, string args)
        {
            Config.removeAdmin(args);
            publishSystemMessage($"{args} removed from admin list");
        }

        private void listAdmin(Farmer fromPlayer, string args)
        {
            publishPrivateMessage(fromPlayer, "Admin list:");
            foreach (ModConfig.SavedConfig.Admin admin in Config.json.admins.ToList())
            {
                publishPrivateMessage(fromPlayer, $"- {admin.name} ({admin.id})");
            }
        }
        private void listPlayers(Farmer fromPlayer, string args)
        {
            publishPrivateMessage(fromPlayer, "Player list:");
            foreach (Farmer farmer in getAllPlayers())
            {
                publishPrivateMessage(fromPlayer, $"- {farmer.Name} ({farmer.UniqueMultiplayerID}), {(farmer.isActive() ? "online" : "offline")}");
            }
        }

        private void buildCabin(Farmer fromPlayer, string args)
        {
            // Take a random pick by default
            string[] cabinTypes = { "Plank", "Log", "Stone" };
            string cabinType = cabinTypes[(new Random()).Next(cabinTypes.Length)];

            if (args.ToLower() == "log")
            {
                cabinType = "Log";
            }
            else if (args.ToLower() == "stone")
            {
                cabinType = "Stone";
            }
            else if (args.ToLower() == "plank")
            {
                cabinType = "Plank";
            }

            if (Game1.getFarm().buildStructure(new BluePrint($"{cabinType} Cabin"), new Vector2((float)(fromPlayer.getTileX() + 1), (float)fromPlayer.getTileY()), Game1.player))
            {
                Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
                publishSystemMessage("Cabin created");
            }
            else
            {
                publishSystemMessage("Couldn't place cabin - is stuff in the way?");
            }

        }
    }
}
