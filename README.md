# wvw_autodemo
Based on LennyPenny's; updated to work on the newest steam/game versions and mostly rewritten.

This tool will automatically record POV demos for you **when the freeze time period begins**. 

It will save them to: `Counter-Strike Global Offensive\csgo\pov\year\month\day\map_hour_minute_second.dem`, to make it easy to cross-reference with Shadowplay clips.

![https://i.imgur.com/0oOe4sF.png](https://i.imgur.com/0oOe4sF.png)
## VAC
The program doesn't read/write to the game's memory at all. It uses Valve's own [GSI](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration) api to retrieve information about the current game state and uses the Windows [SendMessage](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage) function to execute console commands, to understand the latter read [this](https://github.com/perilouswithadollarsign/cstrike15_src/blob/f82112a2388b841d72cb62ca48ab1846dfcc11c8/engine/sys_mainwind.cpp#L774).

## DISCLAIMER
You should run the program at least once before opening up the game to make sure that `csgo\cfg\gamestate_integration_autodemo.cfg` gets written properly, after that you are free to open it mid-game.

## Download
Download it [here](https://github.com/iniw/csgo-AutoDemo/releases).