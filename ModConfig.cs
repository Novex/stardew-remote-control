using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using static RemoteControl.Utilities;

namespace RemoteControl
{
    public class ModConfig
    {
        public class SavedConfig
        {
            public class Admin
            {
                public long id;

                public string name;
            }

            public bool everyoneIsAdmin { get; set; } = false;
            public List<Admin> admins { get; set; } = new List<Admin>();
            public bool shouldAssignAdminToFirstCabinFarmer { get; set; } = true;
        }

        private IMonitor Monitor;

        private IModHelper Helper;

        public SavedConfig json;

        public ModConfig(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;

            json = Helper.ReadConfig<SavedConfig>();
        }

        public void assignFirstAdminIfNeeded()
        {
            if (!json.shouldAssignAdminToFirstCabinFarmer)
            {
                return;
            }

            foreach (Farmer farmer in getAllPlayers())
            {
                Monitor.Log($"Assigning first farmer {farmer}", LogLevel.Debug);
                Monitor.Log($"{farmer.UniqueMultiplayerID} = farmhand id", LogLevel.Debug);
                Monitor.Log($"{farmer.Name} = farmhand name", LogLevel.Debug);

                json.shouldAssignAdminToFirstCabinFarmer = false;
                addAdmin(farmer);   // This will save both settings

                break;
            }
        }

        // Add an admin
        public void addAdmin(Farmer farmer)
        {
            if (farmer is null)
            {
                return;
            }

            Monitor.Log($"Adding {farmer.Name} ({farmer.UniqueMultiplayerID}) to admin list", LogLevel.Debug);

            SavedConfig.Admin newAdmin = new SavedConfig.Admin();

            newAdmin.id = farmer.UniqueMultiplayerID;
            newAdmin.name = farmer.Name;

            json.admins.Add(newAdmin);
            Helper.WriteConfig<SavedConfig>(json);
        }

        // Remove admins by either username or id (so they don't have to be online)
        public void removeAdmin(string farmerDetails)
        {
            Monitor.Log($"Trying to remove {farmerDetails} from admin list", LogLevel.Debug);

            foreach (SavedConfig.Admin admin in json.admins.ToList())
            {
                if (admin.id.ToString() == farmerDetails || admin.name == farmerDetails)
                {
                    Monitor.Log($"Removing {admin.name} ({admin.id}) from admin list", LogLevel.Debug);
                    json.admins.Remove(admin);
                    Helper.WriteConfig<SavedConfig>(json);
                    return;
                }
            }
        }

        public bool isAdmin(Farmer fromPlayer)
        {
            if (isHost(fromPlayer) || json.everyoneIsAdmin)
            {
                return true;
            }

            foreach (SavedConfig.Admin admin in json.admins.ToList())
            {
                if (admin.id == fromPlayer.UniqueMultiplayerID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
