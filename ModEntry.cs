using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using static RemoteControl.Utilities;

namespace RemoteControl
{
    public partial class ModEntry : Mod
    {
        private ModConfig Config;
        static List<ChatMessage> chatMessages = new List<ChatMessage>();
        private bool shouldQuit = false;
        
        public override void Entry(IModHelper helper)
        {
            Utilities.Helper = helper;
            this.Config = new ModConfig(Monitor, helper);

            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
            helper.Events.GameLoop.Saved += this.OnSaved;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Menus.ChatBox), nameof(StardewValley.Menus.ChatBox.receiveChatMessage)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.receiveChatMessage_Postfix))
            );

            helper = this.Helper;
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            foreach (ChatMessage chatMessage in chatMessages.ToList())
            {
                if (chatMessage.chatKind == ChatMessage.ChatKinds.ChatMessage || chatMessage.chatKind == ChatMessage.ChatKinds.PrivateMessage)
                {
                    Farmer farmer = getFarmer(chatMessage.sourceFarmer);
                    parseCommand(farmer, chatMessage.message);
                }
                
            }

            chatMessages.Clear();

            Config.assignFirstAdminIfNeeded();
        }
        private void parseCommand(Farmer fromPlayer, string message)
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
            } catch { }

            if (isHost(fromPlayer))
            {
                parseHostCommand(command, args);
            }

            if (Config.isAdmin(fromPlayer))
            {
                parseAdminCommand(fromPlayer, command, args);
            }

            parseGeneralCommand(fromPlayer, command, args);
        }

        // Commands that can only ever be executed on the host
        private void parseHostCommand(string command, string args)
        {
            switch (command)
            {
                case "shutdown":
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
                    break;
                default:
                    break;
            }
        }

        // Commands that can be executed by anyone with admin
        private void parseAdminCommand(Farmer fromPlayer, string command, string args)
        {
            switch (command)
            {
                case "admin":
                    Config.addAdmin(FirstFarmerByName(args));
                    publishSystemMessage($"{args} added to admin list");
                    break;
                case "unadmin":
                    Config.removeAdmin(args);
                    publishSystemMessage($"{args} removed from admin list");
                    break;
                case "listadmin":
                case "adminlist":
                    publishPrivateMessage(fromPlayer, "Admin list:");
                    foreach (ModConfig.SavedConfig.Admin admin in Config.json.admins.ToList())
                    {
                        publishPrivateMessage(fromPlayer, $"- {admin.name} ({admin.id})");
                    }
                    break;
                case "buildcabin":
                case "cabin":
                case "newcabin":
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

                    break;
                default:
                    // Fall back to just having the host run a standard command
                    Game1.chatBox.textBoxEnter($"/{command} {args}");
                    break;

            }
        }

        // Commands that can be executed by everyone
        private void parseGeneralCommand(Farmer fromPlayer, string command, string args)
        {
            switch (command)
            {
                case "kitz":
                    publishSystemMessage("kitz.. esmllz :O");
                    break;
                // Do something??
                default:
                    break;

            }
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            if (!shouldQuit)
            {
                return;
            }

            // Set the quit flag, the game should pick this up
            Game1.quit = true;
        }

        internal static void receiveChatMessage_Postfix(long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message)
        {
            ChatMessage msg = new ChatMessage();

            msg.sourceFarmer = sourceFarmer;
            msg.chatKind = (ChatMessage.ChatKinds)chatKind;
            msg.language = language;
            msg.message = message;

            chatMessages.Add(msg);
        }
    }
}
