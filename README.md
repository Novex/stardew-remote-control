# Remote Control mod for Stardew Valley

This is a mod for multiplayer servers that allows other farmhands to do actions as if they were the host. Used in the [Stardew Multiplayer Docker](https://github.com/Novex/stardew-multiplayer-docker) server alongside the [Always on Server mod](https://github.com/funny-snek/Always-On-Server-for-Multiplayer)

Farmhands can be added/removed as admins which lets them run special commands. The host always has admin.

## Admin Commands

| Command | Description | 
|--- |--- |
| <pre>!admin &lt;farmhand name></pre> | Add the farmhand to the list of admins |
| <pre>!unadmin <farmhand name/multiplayer id></pre> | Remove the farmhand from the list of admins |
| <pre>!listadmin</pre> | List the name and multiplayer id's of all admins |
| <pre>!cabin [plank/log/stone]</pre> | Build a cabin to the right of the farmhand. It doesn't cost anything, but the area needs to be clear. If no cabin type is specified then one will be picked randomly |
| <pre>!<other_commands></pre> | Will tell the host to run `/<other_command>`, though you won't be able to see the response (todo: fix this). Eg. `!movebuildingpermission on` will allow all players to move buildings |

## config.json
```
{
  "everyoneIsAdmin": false,
  "admins": [],
  "shouldAssignAdminToFirstCabinFarmer": true
}
```

| Command | Description |
|--- |--- |
| everyoneIsAdmin | Allow everyone to use admin commands, useful for trusted servers between friends |
| admins | A list of admins identified by their multiplayer id and name, eg `[{id: 123456789, name: "Sebbity"}, {id: 456781234, name: "Kitz"}]` |
| shouldAssignAdminToFirstCabinFarmer | Assign admin to the farmer that takes the first cabin, useful if you set up a server and don't want to log in to assign an admin (since you'll usually be the first one that uses the server) |

## Host Commands
There are also commands that can only be used on the host

| Command | Description |
|--- |--- |
| <pre>!shutdown</pre> | Rewind time a day, kick all connected players, put the host to sleep and then quit the game after saving the game. Used to save progress if the server needs to restart for whatever reason |