3D TANKS MULTIPLAYER v0.0.1

The online multiplayer demo intended for use with BigParser has reached a point where it is possible, albeit annoying, to play "3D Tanks" across LAN.

FEATURES UNIMPLEMENTED ON MULTIPLAYER CLIENT BUILD:
-game state UI (title screen, round winner, game winner)
-trail dust effects
-shot power indicators
-some sound effects
-tank color variations

All aforementioned features are attainable by the project's deadline. It may be best to put them on hold whilst the game session structure undergoes multiplayer-specific modifications. The open-source project '3D-Tanks' is hard-coded as free-for-all elimination. Audiences may find team deathmatch more compelling.


How to run:

1) First, run the "Tank Game Server.exe" file (BigParserRPCFramework->ProjectFolder->ServerBuild->Tank Game Server.exe). 
2) A command window will pop up and start printing a bunch of lines. 
3) Windows will probably ask for permission. Allow it to communicate on private networks.
note: if you see it stop at a line mentioning "socket exception", then the port that the build attempts to access (port 600) is taken by another server application. You probably have launched the server's Unity editor project on that port already! Turn the editor off and wait a few minutes before trying again.

4) Then, on a computer using the same local area network (same wifi name) as the server, run "Tank Game Client" (BigParserRPCFramework->ProjectFolder->ClientBuild->Tank Game Client.exe). 
5) This will launch the game. If the server is running successfully, then the game will display a login screen. 
note: If you see no UI for ~five seconds, followed by a screen that asks for an IP address and port, then you've probably failed to launch the server
6) Leave username blank, click CONTINUE, leave password blank, click LOGIN.
7) Alt-Tab out and repeat steps 4-7 to launch another instance of the game, or launch another instance on a different computer using the same wifi network. A few seconds will pass after two players have connected, and you will be able to battle with the tanks same as the original build.

TLDR: run the server, then run a couple clients.

