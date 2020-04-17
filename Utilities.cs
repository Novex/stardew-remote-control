using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControl
{
    public class Utilities
    {
        public static IModHelper Helper;

        public static bool isHost(Farmer farmer)
        {
            return Game1.player == farmer;
        }

        public static string getPlayerName(long sourceFarmerId)
        {
            return getPlayerName(getFarmer(sourceFarmerId));
        }

        public static string getPlayerName(Farmer sourceFarmer)
        {
            string str = Game1.content.LoadString("Strings\\UI:Chat_UnknownUserName");

            if (!(sourceFarmer is null))
                str = ChatBox.formattedUserName(sourceFarmer);

            return str;
        }

        public static Farmer getFarmer(long farmerId)
        {
            Farmer farmer = null;

            if (farmerId == Game1.player.UniqueMultiplayerID)
                farmer = Game1.player;

            if (Game1.otherFarmers.ContainsKey(farmerId))
                farmer = Game1.otherFarmers[farmerId];

            return farmer;
        }

        public static Farmer FirstFarmerByName(string name)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.name.ToString() == name)
                {
                    return farmer;
                }
            }

            return null;
        }

        // Sends a public chat message
        public static void publishSystemMessage(string msg)
        {
            // send to everyone else
            Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, msg, Multiplayer.AllPlayers);

            // also show the message in our own message box
            Game1.chatBox.receiveChatMessage(Game1.player.UniqueMultiplayerID, (int)ChatMessage.ChatKinds.UserNotification, LocalizedContentManager.CurrentLanguageCode, msg);
        }

        // Send a PM
        public static void publishPrivateMessage(Farmer player, string msg)
        {
            // send to everyone else
            Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, msg, player.UniqueMultiplayerID);

            // also show the message in our own message box
            Game1.chatBox.receiveChatMessage(Game1.player.UniqueMultiplayerID, (int)ChatMessage.ChatKinds.PrivateMessage, LocalizedContentManager.CurrentLanguageCode, msg);
        }
    }
}
