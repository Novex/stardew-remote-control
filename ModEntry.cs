using System.Collections.Generic;
using System.Linq;
using Harmony;
using RemoteControl.Commands;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using static RemoteControl.Utilities;

namespace RemoteControl
{
    public partial class ModEntry : Mod
    {
        private ModConfig Config;
        static List<ChatMessage> chatMessages = new List<ChatMessage>();

        HostCommands hostCommands;
        UserCommands userCommands;
        AdminCommands adminCommands;

        public override void Entry(IModHelper helper)
        {
            Utilities.Helper = helper;
            this.Config = new ModConfig(Monitor, helper);

            hostCommands = new HostCommands(Config);
            userCommands = new UserCommands(Config);
            adminCommands = new AdminCommands(Config);

            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
            helper.Events.GameLoop.Saved += this.OnSaved;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Menus.ChatBox), nameof(StardewValley.Menus.ChatBox.receiveChatMessage)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.receiveChatMessage_Postfix))
            );

            helper = this.Helper;
        }

        public override object GetApi()
        {
            return new ModApi(hostCommands.AddCommand, adminCommands.AddCommand, userCommands.AddCommand);
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Config.assignFirstAdminIfNeeded();

            // Process chat messages that came in
            foreach (ChatMessage chatMessage in chatMessages.ToList())
            {
                if (chatMessage.chatKind == ChatMessage.ChatKinds.ChatMessage || chatMessage.chatKind == ChatMessage.ChatKinds.PrivateMessage)
                {
                    Farmer farmer = getFarmer(chatMessage.sourceFarmer);

                    hostCommands.ParseCommand(farmer, chatMessage.message);
                    adminCommands.ParseCommand(farmer, chatMessage.message);
                    userCommands.ParseCommand(farmer, chatMessage.message);
                }
                
            }

            chatMessages.Clear();
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            hostCommands.OnSaved(sender, e);
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
