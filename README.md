# Remote Control mod for Stardew Valley

This is a mod for multiplayer servers that allows other farmhands to do actions as if they were the host. Used in the [Stardew Multiplayer Docker](https://github.com/Novex/stardew-multiplayer-docker) server alongside the [Always on Server mod](https://github.com/funny-snek/Always-On-Server-for-Multiplayer)

Farmhands can be added/removed as admins which lets them run special commands. The host always has admin. Only the host needs this installed for multiplayer.

It's mostly used for quickly making cabins for new players, but can be extended by these mods:
  - (open a PR to add a link if you use this!)

Note: If you're connecting to the multiplayer server via a direct LAN IP then players connecting can choose any farmhand they want - including the admin ones! In this case, it would be better to admin no one and just perform admin commands from the host, or if everyone's trustworthy then set the `everyoneIsAdmin` config to true so anyone can do commands.

If connecting via the Invite Code then players can only pick their own player, so everything works far more securely.

## config.json
```
{
  "everyoneIsAdmin": false,
  "admins": [],
  "shouldAssignAdminToFirstCabinFarmer": false
}
```

| Command | Description |
|--- |--- |
| everyoneIsAdmin | Allow everyone to use admin commands, useful for trusted servers between friends |
| admins | A list of admins identified by their multiplayer id and name, eg `[{id: 123456789, name: "Sebbity"}, {id: 456781234, name: "Kitz"}]` |
| shouldAssignAdminToFirstCabinFarmer | Assign admin to the farmer that takes the first cabin, useful if you set up a server and don't want to log in as the host to assign an admin (since you'll usually be the first one that uses the server) |

### Admin Commands

| Command | Description | 
|--- |--- |
| <pre>!admin &lt;farmhand name></pre> | Add the farmhand to the list of admins |
| <pre>!unadmin &lt;farmhand name/multiplayer id></pre> | Remove the farmhand from the list of admins |
| <pre>!listadmin</pre> | List the name and multiplayer id's of all admins |
| <pre>!cabin [plank/log/stone]</pre> | Build a cabin to the right of the farmhand. It doesn't cost anything, but the area needs to be clear. If no cabin type is specified then one will be picked randomly |
| <pre>!<other_commands></pre> | Will tell the host to run `/<other_command>`, though you won't be able to see the response (todo: fix this?). Eg. `!movebuildingpermission on` will allow all players to move buildings |

### User Commands
These are commands everyone can use - there are none yet (any suggestions?)

| Command | Description |
|--- |--- |

### Host Commands
There are also commands that can only be used on the host

| Command | Description |
|--- |--- |
| <pre>!shutdown</pre> | Rewind time a day, kick all connected players, put the host to sleep and then quit the game after saving the game. Used to save progress if the server needs to restart for whatever reason |

## API
Other mods can use the mod's [SMAPI Integration](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Using_an_API) to register their own commands to be run.

Commands are case-insensitive. Ideally try to keep the commands unique - if multiple commands are registered only one will be run. If the same command trigger is registered for both an admin and a user then both will be run.

You can use the following interface:

```
    public interface IRemoteControlApi
    {
        // Add a command that can only be run by the host
        void AddHostCommand(string commandTrigger, Action<Farmer, string> functionToRun);

        // Add a command that can only be run by an admin player
        void AddAdminCommand(string commandTrigger, Action<Farmer, string> functionToRun);

        // Add a command that can be run by anyone
        void AddUserCommand(string commandTrigger, Action<Farmer, string> functionToRun);

        // Send a message from the host to everyone
        void publishSystemMessage(string msg);

        // Send a PM from the host to a specific player
        void publishPrivateMessage(Farmer toPlayer, string msg);
    }
```

And an example on how to use it in your code:
```
namespace HelloCommands
{
    public partial class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			IRemoteControlApi api = Helper.ModRegistry.GetApi<IRemoteControlApi>("Sebbity.RemoteControl");

            // No commands will be registered if the mod isn't available
			if (api != null)
			{
                // This will respond to everyone for !hello
                api.AddUserCommand("hello", helloUser);

                // These will respond to !helloadmin and !adminhello
				api.AddAdminCommand("helloadmin", helloAdmin);
				api.AddAdminCommand("adminhello", helloAdmin);
			}
		}

        // Say hello to the user that sent the command
        private void helloUser(Farmer fromPlayer, string args)
        {
            IRemoteControlApi api = Helper.ModRegistry.GetApi<IRemoteControlApi>("Sebbity.RemoteControl");

            if (api == null)
                return;

            // Send the hello player message publicly
            api.publishSystemMessage($"Hello {fromPlayer.Name}!");
        }

        // Let admins tell the host to say hello to anyone they want (eg. !helloadmin seb => Hello seb!)
        private void helloAdmin(Farmer fromPlayer, string args)
        {
            IRemoteControlApi api = Helper.ModRegistry.GetApi<IRemoteControlApi>("Sebbity.RemoteControl");

            if (api == null)
                return;

            // Send the hello message publicly
            api.publishSystemMessage($"Hello {args}!");

            // Also send a private message back to the admin 
            api.publishPrivateMessage(fromPlayer, $"Some secret stuff you don't want everyone else knowing ;)");
        }
    }
}
```