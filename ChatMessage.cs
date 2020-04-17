using StardewValley;

namespace RemoteControl
{
    public class ChatMessage
    {
        public enum ChatKinds
        {
            ChatMessage,
            ErrorMessage,
            UserNotification,
            PrivateMessage
        }

        public long sourceFarmer;
        public ChatKinds chatKind;
        public LocalizedContentManager.LanguageCode language;
        public string message;
    }
}
