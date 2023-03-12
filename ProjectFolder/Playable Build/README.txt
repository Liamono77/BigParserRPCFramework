3D TANKS MULTIPLAYER v0.1.0

The online multiplayer demo intended for use with BigParser has reached a point where it is possible to play "3D Tanks" across LAN. 

NEW
-Client UI (various)
-Major structure alterations; gamemode no longer hardcoded
-Timer-based player spawning; no more server softlock.

FEATURES UNIMPLEMENTED ON MULTIPLAYER CLIENT BUILD:
-trail dust effects
-shot power indicators
-some sound effects
-tank color variations

All aforementioned features are attainable by the project's deadline. Should be able to get to them soon. Server structure is nearly ready for more committal approach to player prefabs. Ideally, players will see who killed them through in-game spectator mode. 


How to run:

1) First, run the "Tank Game Server.exe" file (BigParserRPCFramework->ProjectFolder->PlayableBuild->ServerBuild->Tank Game Server.exe). 
2) A command window will pop up and start printing a bunch of lines. 
3) Windows will probably ask for permission. Allow it to communicate on private networks.
note: if you see it stop at a line mentioning "socket exception", then the port that the build attempts to access (port 600) is taken by another server application. You probably have launched the server's Unity editor project on that port already! Turn the editor off and wait a few minutes before trying again.

4) Then, on a computer using the same local area network (same wifi name) as the server, run "Tank Game Client" (BigParserRPCFramework->ProjectFolder->PlayableBuild->ClientBuild->Tank Game Client.exe). 
5) This will launch the game. If the server is running successfully, then the game will display a login screen. 
note: If you see no UI for ~five seconds, followed by a screen that asks for an IP address and port, then you've probably failed to launch the server
6) Leave username blank, click CONTINUE, leave password blank, click LOGIN.
7) Player tank should spawn in and be controllable immediately. 
8) Repeat steps 4-7 to launch more clients. Clients on other computers can join the server if on the same LAN.

TLDR: run the server, then run a couple clients.

