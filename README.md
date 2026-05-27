# About the Project

Turnscape is a 2D, simplistic, low-graphics style turn-based battle game with item exchange mechanics. The main idea of the game is to combine combat with a simple player-driven economy where acquiring items is the main way to progress.

Players earn money through battles, and they can use them to buy, items from others, they can sell items they have. This makes progression depend not only on fighting, but also on the player interaction.

The main idea of Turnscape is multiplayer interaction, valuable items, and simple trading between players. The focus is on making items useful and creating more interaction between players outside of battles.

This project was created for university studies (Programming Systems course) as a way to practice building a working game system and applying programming concepts in practice.



## Main Features

The features of Turnscape are focused on turn-based combat, item management, and simple multiplayer interaction through menus and game scenes.

- Main game scenes

    - Main Menu – the starting point of the game

    - Base Scene – used for inventory management, equipment, and shop interactions

    - Combat Scene – where battles between players take place after matchmaking

- Turn-based combat system – Players do not control movement in real-time. Instead, battles are decided by choosing attack types and actions through UI buttons. The gameplay is focused on strategy and decision-making during turns. Multiplayer PvP focus – The current version supports 1v1 player vs player battles.

- Scene and UI navigation – The game is fully menu-based. Players navigate through different scenes such as the main menu, base scene, and combat scene using UI controls instead of direct movement.

- Inventory system – Players can manage their items using an interactive drag-and-drop system. Items can be equipped, stored, or prepared for trading. Equipped items are visible on the player model.

- Item economy (shop and trading concept) – Players can earn money from battles and use it to buy or sell items. This supports the core idea of item value and player-driven progression.

- Settings system – Players can adjust basic settings such as resolution, fullscreen mode, and sound volume.

- Backend system – The game uses a server-side backend with a database to store player data, items, and match-related information. This allows multiplayer functionality, data persistence, and item economy features to work across different players.



## Technologies Used

Turnscape was developed using the Unity game engine and the C# programming language. C# was used both on the client side (Unity) and on the server side, which allowed the entire system to be built using a single main language and made development more consistent.

The project was developed in Visual Studio as the main development environment. Git and GitHub were used for version control, allowing the team to collaborate, track changes, and manage the codebase effectively.

The development process followed the Scrum methodology. Jira was used for task management and planning, which helped structure the work into clear sprints, track progress, and improve team coordination throughout the project.

In addition to the core technologies, Unity tools were used for UI development and physics systems. The server-side implementation was also written in C#, ensuring a consistent architecture between the client and backend systems.



## Game Flow

Turnscape follows a simple loop based on preparation, combat, and progression.

- At the start, the player enters the main menu and can navigate to their base or start matchmaking.

- In the base scene, the player manages their inventory, equips items, and can interact with the shop.

- When matchmaking starts, the player is placed into a turn-based combat scene against another player.

- During combat, players take turns choosing actions (attacks or abilities) through the UI.

- The outcome of battles depends on strategy, item usage, and player decisions.

- After a battle:

    - If the player wins, they receive money and/or items.

    - If the player loses, they receive no rewards or lose progress depending on the system rules.

- Progression is based on improving gear and accumulating better items over time, which increases the player’s strength in future battles.

- The overall game loop is:
 preparation → combat → rewards → improvement → next combat.



## Controls

Turnscape is fully UI-based and controlled with the mouse. All interactions, including menus, combat actions, inventory, and shop, are done through clickable UI elements.



# Project Structure

The project is split into a Unity client and a separate server-side application, each with its own structure and responsibilities.

On the Unity side, the project is organized in a typical Unity structure. The Assets folder contains all game content. Inside it, Scripts holds all gameplay and UI logic written in C#. Scenes contain the different game scenes such as Main Menu, Base, and Combat. UI contains prefabs and elements used for menus and interfaces. Textures store static visual assets, while Resources / StreamingAssets are used for dynamically loaded content such as item icons and other runtime-loaded data.

The server-side project is separated from Unity and contains its own C# codebase. It is responsible for handling backend logic such as player data, item management, and multiplayer communication. The server structure is organized into logical layers such as controllers, services, and data access, separating request handling from core game logic and database operations.



## Architecture

Turnscape is built as a modular Unity-based multiplayer system where gameplay, UI, and backend communication are separated into distinct layers. The architecture is designed to keep game logic centralized, while allowing UI and server interactions to remain loosely coupled and replaceable.



## Unity Game Architecture

The core of the game is implemented in Unity using MonoBehaviour components attached to scene objects. This allows systems to use Unity’s lifecycle methods (Start, Update, coroutines) while remaining modular and reusable across scenes.

At the center of the gameplay layer is the GameManager, which coordinates major systems such as combat, inventory, shop, and player state. Instead of direct script-to-script communication, most systems interact through manager classes, reducing tight coupling between gameplay features.

A key structural element is the custom LoaderBehaviour system, which manages all persistent game data through a controlled lifecycle:

- Download (async) – fetch data from server

- Load (sync) – initialize local and scene data

- Apply (sync) – finalize initialization after loading

- Unload (sync) – prepare data for saving or scene exit

- Upload (async) – send updated data to server

These stages always execute in a strict order, ensuring consistent state transitions. Each LoaderBehaviour can also depend on others, forming a controlled dependency chain that prevents systems from loading or unloading out of order.

All LoaderBehaviour instances act as singletons, ensuring a single source of truth for each type of game data and preventing duplicate or conflicting states.

Existing managers:

- Game manager - controls all manager loadings, scene changes

- Inventory manager - downloads and holds player inventory data, synchronizes it with the server

- Statistics manager - locally calculates statistics for UI according to other manager (inventory manager) data. Doesn’t apply it in combat, as it is realised on server side for security. Depends on the inventory manager.

- Dresser manager - manages player model and equipped item display on the model. Depends on the inventory manager.

- Screen manager - provides screen resolution changes, fullscreen mode, safe screen resizing.

- Toggle manager - makes sure menu panels are opened only one at the time.

- Audio manager - manages sfx audio events.

- Music manager - manages background music.

- Queue service (base scene) - provides a match queue.

- Fight manager (combat scene) - controls combat mechanics and synchronizes it with the server (other player).



## Server & Networking Layer

The backend layer handles multiplayer interactions, matchmaking, and persistent data storage. Communication between the client and server is asynchronous, mainly driven through the LoaderBehaviour download/upload stages and dedicated networking scripts.

Key responsibilities of the server layer include:

- player matchmaking and queue management

- storing and retrieving player data

- synchronizing player state between sessions

- handling trading and economy-related transactions

On the client side, lightweight DTO objects (e.g. UpdatePosDto) are used to transfer structured data efficiently between Unity and the backend.

The server is treated as the single source of truth for persistent game state, while the client maintains a local runtime representation for gameplay responsiveness.

## UI Plan and Navigation Manual

All visible assets are implemented as UI elements instead of sprites. This allows easier adaptation to different screen sizes and resolutions. Each scene has its own UI elements, which stay visible unless covered by opened panels.





When the game launches, the player first sees a loading panel and then enters the Main Menu scene. The loading panel appears between scene loads. The loading panel consists of a circular loading bar (animated), percentage of stage loaded, loading stage indicator.



Upon finishing the game loading process and entering the main menu scene, the player can log into an existing account, create a new account, open settings, or quit the game. Pressing the Log In or Sign Up buttons opens a panel where the player can enter a username and password. Besides that we can see the quit game button and settings button.

After pressing log in button or sign up button a panel like this will open:



We can see here Username input field, Password input field, Log in/Sign up confirm button.



After successfully logging in, the available buttons change to Play and Log Out. Pressing Play switches the game to the Base Scene, starting the game.

 Pressing settings button opens a panel:



Here the player can change fullscreen mode, resolution dropdown selection, music volume slider, SFX volume slider.

Upon pressing play button in main menu scene, base scene opens:



In the Base Scene, the player can manage equipment, inventory, and matchmaking. The player statistics panel is shown on the left side of the screen, while the player model is positioned in the middle. Equipment slots surrounding the player model are used for equipping items. The inventory grid is displayed on the right side of the screen. Above the inventory grid is the shop button. The menu button is located in the top-left corner, and the Join Combat button is placed in the bottom-right corner.

The inventory consists of item slots that can contain items. Each item is visually represented by an item icon displayed inside a slot. Items can be moved by clicking and holding the left mouse button on an item icon and dragging it to another slot. Only items with the correct category fit certain equipment slots.

Hovering over an item displays additional information such as its name, level, and statistics.



The shop can be opened using the shop button above the inventory. The shop appears as a panel over the current scene and displays a buy/sell item list. The player can switch between buying and selling modes using the mode switch button, which allows listing owned items for sale. When in sell mode, the player can adjust item selling options using the sell price selection. The shop can be closed using the close button.



The menu button in the top-left corner opens another panel containing settings, a Back to Main Menu option, and a Quit Game option.



Pressing the Join Combat button in the bottom-right corner places the player into the matchmaking queue. While waiting for an opponent, a queue panel is displayed showing queue status, and the player may leave the queue at any time using the Leave Queue button. Once another player is found, the game automatically switches to the Combat Scene.



In the Combat Scene, the player model is displayed on the left side while the enemy is shown on the right side. Health bars above both characters display remaining health. The current turn is shown at the top-center of the screen. During the player’s turn, attack buttons appear at the bottom of the screen. The player may choose between a Heavy Attack, which deals more damage but has a higher chance to miss, or a Light Attack, which is weaker but has a higher chance of hitting. The menu button is also available during combat.

Whenever damage is dealt, a damage number briefly appears near the affected character. Damage text disappears after a short time.





Combat ends when one player’s health reaches zero. A result panel then appears showing whether the player won or lost the match. If the player wins, they receive an in-game currency reward, which is displayed in the panel. The player can then return to the Base Scene using the provided return button.



The Combat Scene also contains a menu button that opens the Combat Menu Panel, which includes a Settings button, a Leave Combat button, and a Quit Game button.



# Installation and Running the Project (Unity)

## Requirements:

Before running the project, make sure you have:

- Unity Version: Unity 6000.3.7f1 LTS

- Operating system: Windows 10/11 or Linux (MacOS should work but untested)

- Git installed on your computer

- Recommended: Code editor (Visual studio, Rider or etc.) for editing scripts.

## Downloading the Project

1. Clone the repository using Git:

 ```git clone https://github.com/Armisb/turnscape.git```

2. Open the cloned folder:

 ```cd turnscape```

## Opening the project in Unity

1. Open Unity Hub

2. Click Add Project

3. Select the game folder of the cloned folder (turnscape/turnscape)

4. Choose the correct Unity version

5. Click Open

## Running the Game

- Play mode

    - To run the game inside the Unity Editor:

        1. Open the “MainMenuScene” From the Scenes folder

        2. Press the Play button at the top of the Unity Editor

- Building the Game

    - To create a standalone build:

        1. Open File -> Build Settings

        2. Select your target platform

        3. Click Build

        4. Choose a destination folder

    After the build finishes, run the generated executable file.

## Troubleshooting

If Unity reports missing packages:

- Open Window -> Package Manager

- Reinstall missing dependencies

# Testing

## Problems and Solutions

- Protection Calculations

    - Problem: Armor-type items were not correctly calculating protection stats based on item level.

    - Cause: Level scaling was only implemented for weapon-type items.

    - Solution: Added level-based stat calculations for armor-type items.

    - Result: Protection statistics are now calculated correctly for all applicable items.



- Queue System

    - Problem: The initial implementation of the matchmaking queue system was not functioning correctly.

    - Cause: The system was built using an incorrect approach, with no reliable way to determine when a match could be created.

    - Solution: Implemented an additional service that runs on a timer and checks every second whether enough players are available to create a match.

    - Result: The server was able to successfully create matches and send match events to players.

- Store Inventory Item Type Glitch

    - Problem: Items in the inventory could have their inventory type changed by another service.

    - Cause: The updatePositions function did not account for the store inventory type and reset items back to the default player inventory type when called.

    - Solution: Added a conditional check to ignore items with the store inventory type.

    - Result: Items stored in the shop inventory no longer reverted to the incorrect inventory type.

# Future Improvements



Attack calculations improvement – currently, attacks are calculated almost randomly. Calculation functions should be added to determine attack outcomes more logically and in a balanced way.

Item distribution system – currently, there is no proper method for adding items into the game other than through the admin. It would be a great improvement if players could obtain items through in-game interactions without admin interference.

Container opening system – a feature could be added where players are able to open containers that contain different items with specific drop chances. These containers could be purchased using in-game currency, making the system more engaging and rewarding for players.

Shop functionality – more statistics should be added to the shop, such as the item listing date and other useful information. Additional features like item filtering, searching, and sorting should also be implemented to improve usability. Furthermore, players should have the ability to remove items from the shop if they no longer want to sell them.

Match history – a match history feature could be added, allowing players to review their previous fights, track their performance, and analyze outcomes. This would help them make strategic adjustments to their layout based on past battles and the layouts of their opponents.

Match ending/leaving fix – currently, the server and game do not properly handle situations where a player leaves or experiences connection issues during a match, causing the match to hang. This issue should be fixed by implementing proper match termination and reconnection logic to ensure the game ends or recovers correctly in such cases.



# Project Creators

This project was created by Turnscape Team:

- Tautvydas Tauras

- Nojus Žeimys

- Arminas Bilevičius

- Deividas Išganaitis

# GitHub Repository

Source code and project files are available on GitHub:

```https://github.com/Armisb/turnscape```

## License

This project is licensed under the MIT License.

You are free to use, copy, modify, and distribute this project, including for educational and personal use, as long as the original authors are credited.

