# Welcome to DSTools
DSTools is a Unity Editor plugin that aims to create an integrated modding solution for Dark Souls 1 (currently only Prepare to Die Edition is supported), Dark Souls 3, Bloodborne, and Sekiro. Currently the tools support editing existing maps for the games and allow you to move enemies and objects around, but aims to eventually allow the creation of completely new levels.
## Features
* Import object, character, and map assets from Dark Souls 1, Dark Souls 3, Bloodborne, or Sekiro into Unity.
* Import an entire map including object and enemy placement into a Unity scene.
* Move, add, or delete enemies and objects in a map with the Unity editor in a mostly what you see is what you get manner.
* Export modified maps directly back to the game in a ready to play manner.
* Edit everything about the Dark Souls maps, including unknown properties.

## Installation and Basic Tutorial
0. Be warned that a lot of disk space is needed. You'll probably want at least 25GB of storage for DS1 modding and 50GB+ for DS3/Sekiro. This is because all the archives need to be unpacked, unity needs to be installed, and there needs to be room for imported assets cached in Unity.
1. Join the Souls modding discord (https://discord.gg/mT2JJjx). Trust us, Dark Souls modding is still difficult, and almost all active modders are in this community and are happy to answer questions.
2. Unpack your game of choice that you are modding. Use [UDSFM](https://www.nexusmods.com/darksouls/mods/1304) for DS1:PTDE and [UXM](https://www.nexusmods.com/darksouls3/mods/286) for DS3.
3. Install Unity 2018.3.6 (free edition works and is what I used). Older and newer versions are not tested and may not work.
4. Create a new project WITH the High-Definition RP (Preview) template. Other templates are not supported.
5. In the new project go to Window->Package Manager and update High Definition RP to the latest version. Older versions cause massive stuttering with complex maps loaded.
6. Go to Edit->Project settings and in the Editor section set the Asset Serialization Mode to Force Binary. This saves a massive amount of disk space and makes reading and writing assets much faster.
7. Download the latest release from the releases section (should be a unity package). Import the package with Assets->Import Package.
8. Open the tools with Window->Dark Souls Tools. Click on Set Game Root Directory and select the exe of your desired game.
9. If modding DS1:PTDE, click on Import DS1 UDSFM Textures. This may take some time, and only needs to be done once.
10. Click on Import Objs, and then Import Chrs. This imports all the models for enemies and objects. Both may take a long time, but only need to be done once per game.
11. Finally you can try and import a map. Create a new scene or delete all the scene models in the default scene (leave all the volume and light stuff). By default, only the low resolution collision models are loaded because it's much faster and still allows you to see the level and place enemies. If you want to import the high resolution collision models (used for leg IK), you can check Load high resolution collision. If you want to import all the level geometry, check Load Map piece models. Note that importing level geometry takes a long time and requires a lot of space especially for DS3 maps. Note for DS3 if you want map textures, you have to import all the TPFBHD files with Import TPFBHD for the map you want to load BEFORE importing the map. these are located in the map/mXX directory (where mXX is your target map) in your game's root directory. You need to import all 4 files.
12. Click import map and select a map. For initial testing, m18_01_00_00 is asylum in DS1, and m40_00_00_00 is Cemetary of Ash in DS3.
13. Try moving an enemy to a new position by clicking on a basic hollow in the viewport and placing it somewhere else.
14. Click Export Map. Backup files will automatically be created so don't worry about that. Start a new game and you should see your enemy moved.

## Dark Souls Modding Crash Course
Once you move an enemy, you may be overwhelmed by the amount of objects and entities in an imported scene, and all the dark souls specific properties. Many things are still unknown, and you are encouraged to experiment around and figure out new things for the community, but here is a basic crash course on some of the things you may encounter.

### Game directory structure:
Inside the unpacked game root directory, you will see the following structure for the game's files:
* **chr/** - Archives for characters and all their models and animations. Characters include the player (c0000), enemies, bosses, NPCs, and even interactables such as the bonfire. Each character is identified by a unique ID in the form of cXXXX. Each character will be named in that form in the editor.
* **obj/** - Archives for objects. Objects are mostly static small things that can be placed in the map. They may have collision, may have simple animations, or may have simple triggerable actions (called object actions or ObjActs). Objects include breakable pots, doors, elevators, crows/birds in the map, and other decorations. Objects are itentified by a 4 (6 in DS3/BB) digit identifier in the form of oXXXX, and are shown as that in the editor.
* **event/** - Event scripts for the maps. These do lots of things, and basically scripts everything that "happens" in the map. These are editable with HotPocketRemix's event tools, so search for those and his tutorial videos for more information on how to mod them. Event scripts refer to entities in the map with an "EventEntityID" that is set for characters placed in the map.
* **map/** - Maps and their assets. Maps also have ids in the form of mAA_BB_00_CC. AA is an area id that shares some assets like textures with other maps with the same area id. BB is a block that has for the most part unique models, collisions, and navimeshes. CC is a variation used for some DS1 and BB maps that are used for very slightly different versions of the same map and use the same assets. For example, Darkroot garden has a variation of the map depending on if you have the DLC or not. Inside the MapStudio directory are msb files for each map, which defines the layouts and placements of everything. This is what the editor imports and modifies.

### MSB basics
MSB (MapStudio Binary) is the file format for maps in the SoulsBorne games. The format has gone through significant changes throughout the games, but follows the same general structure. MSBs have four (more in DS3) different "sections", all of which are imported and editable in the editor:
* **Models** - also called Model Declarations in the editor. These declare all the assets that are loaded for the map. An entry is required here for every asset, and the name of the entry is the id of the asset being loaded.
* **Parts** - These are all the visible entities in the map, and include map pieces, collisions, NPCs, objects, etc. They all have transforms, entity ids for event scripts to reference, and draw/render/backread groups, which determine visibility from certain areas and level streaming of new assets.
* **Regions** - These are invisible points or trigger regions that have a variety of purposes. They define spawn points for you and invadors, regions large special effects such as fog are spawned in, triggers where entering it causes an event to occur such as a boss fight or enemy ambush, and other things.
* **Events** - Not to be confused with event scripts. These are invisible abstract entities that don't have any location (though unity gives it one) that bake in some events directly into the MSB. These can give a treasure to a chest or corpse, regenerate enemies when they die, define spawn points, and define triggerable object actions such as doors and elevators.

You will also encounter parts, which make up the bulk of the map and is the most interesting. Some of the "parts" you may encounter are
* **Map Pieces** - Static map meshes. These are textured 3D models that never move, have no animation, and are stored as an asset for a specific map (i.e. a map piece for one map can't be used in another map without the mesh being directly copied to the new map).
* **Objects** - Objects in the map, referred to with the 4-6 digit object ID. These are smaller pieces that can be animated, can be destructable, and some other things. These can be loaded in any map in theory, but some objects are meant to be used with only one map and has its texture assets stored with a specific map.
* **DummyObjects** - Objects mostly used with cutscenes and are usually invisible
* **NPCs/Enemies** - Characters placed in the map. These are your enemies, npcs, bosses, and bonfires.
* **Dummy NPCs/Enemies** - Characters specific to cutscenes.
* **Collisions** - Havok collision meshes placed in the level. These are what you actually stand on and play on, and the collision mesh you stand on determines many things such as what loads in the background, what things are visible to you (for level of detail), and other things still being figured out.
* **Connect Collisions** - An entity that can be attached to a collision. Standing on a connect collision triggers loading and rendering of a different map, and is used to create the seamless world.
* **Players** - Positions and spawns for your character.

There is a LOT I didn't cover here, so feel free to suggest things to add or ask more questions in our modding discord.

## Tools and Resources
Here is a list of resources that may be helpful while modding:
* [Meowmaritus's gists](https://gist.github.com/Meowmaritus): Tons of documents and references for map IDs, object IDs, and character IDs for all the games. Very useful references when editing maps.
* [Yapped](https://www.nexusmods.com/darksouls3/mods/306?tab=files): Param editor for DS3. Many entities in maps reference params for things, and this allows you to lookup and edit the params for DS3.
* [Yabber](https://www.nexusmods.com/darksouls3/mods/305?tab=files): Soulsborne archive unpacker/repacker. Allows you to unpack and repack all the BND files in the games. Also allows you to extract and repack textures, and convert the game's text fmgs (located in msg/) to xml and back for easy editing.
* [ParamVessel](https://github.com/Meowmaritus/ParamVessel/releases): Param editor for DS1. Does similar things that Yapped does for DS3.
* [DarkScripts2](https://github.com/C-Weinstein/DarkScript-2/releases): GUI based tool for editing event scripts in all the games made by Aintunez and HotPocketRemix. Use this to add scripting to your modded maps. Watch HotPocketRemix's [tutorial series](https://www.youtube.com/watch?v=QXpqNNZBKoU) to learn more about event scripting.


## FAQ
### **Q**: Why did you use Unity for this?
**A**: My ambition for this tool is to eventually turn this into a complete solution for creating new levels from scratch. For a modern game like Dark Souls, this requires an end to end pipeline for importing meshes from a 3D program, placing them and enemies in the map, baking light maps, generating navimeshes, making collision, and baking and exporting all of them. Unity provides an extensible, scriptable editor and a lot of features that can help with baking light maps and doing navimeshes. It isn't perfect and it's pretty awkward to make support another game with an engine super different from Unity, but using it has allowed me to do everything I've done much faster than a from scratch solution.
### **Q**: Why isn't Dark Souls II supported?
**A**: While Dark Souls II's engine is derived from DS1, they made significant changes that makes it quite different from the other games. DS3 in comparision still is similar to DS1 in many ways DS2 is not. DS2 also lacks documentation in the game files that other games had, making figuring out stuff much harder. Also supporting a game in this editor is a LOT of work, and I can't justify the effort to support DS2 enough when I have other things I'm working on (like enabling completely new custom DS1/DS3 maps).
### **Q**: Why is Bloodborne supported then?
**A**: Bloodborne actually has a lot of documentation left in the game files and its engine is based off of Dark Souls 1. I also have a personal interest in Bloodborne modding, and it's similar to Dark Souls 3 that supporting it is much easier than supporting Dark Souls 2.
### **Q**: Where are the tutorials?
**A**: I haven't had time to make any, as it's a lot of work and the editor is still undergoing rapid development. The editor is also not super user friendly right now, as so much work has gone into basic functionality. Tutorials are something I would like to do later, but I am busy with development and the editor is rapidly changing as I iterate through things. If you need help (and you probably will), don't be afraid to ask me or other experienced modders in the discord. If you have a good handle on the editor and would like to make tutorials (video or otherwise), it would be greatly appreciated and I will give a shoutout here.
### **Q**: Can I contribute?
**A**: Absolutely. There's lots of things that can be done to help such as reverse engineering file formats, learning more about the MSB file format by experimenting with unknown fields, making mods to find pain points or bugs, or even writing more tools with C#. If you are proficient in C# and potentially have some Unity experience and want to contribute, ping me on Discord for some ideas of things to do and send me pull requests. I'm open for people with the right skillset to join the project as a core dev as well.
### **Q**: How do I move a bonfire?
**A**: Bonfires have many hidden points that need to be moved in addition to the bonfire "character". There's a player entity and a spawn point region that needs to be moved as well. Use the inspector to search for these objects and move them to new locations as well.

## Copyright:
All editor source code, except for the following exceptions, is Copyright (C) 2019 Katalash. All rights reserved.

DSTools uses Soulsformats by TKGP. Soulsformats is Copyright (C) 2019 TKGP.

DSTools uses MeowDSIO by Meowmaritus. MeowDSIO (and only MeowDSIO under the MeowDSIO directory) is released under the MIT Lisence.

## Shoutouts and Credits:
DSTools was made possible by the dedicated members of the Dark Souls modding community including:
* FloorBelow for contributing Sekiro MSB read/write support.
* TKGP for making Soulsformats, UXM, general reverse engineering, and for answering many questions for me.
* Meowmaritus for making MeowDSIO, reverse engineering, also answering questions.
* Pav for beta testing and helping me with lots of reverse engineering of various aspects of DS3.
* Rivernyxx for early Dark Souls research and 010 templates which were very useful.
