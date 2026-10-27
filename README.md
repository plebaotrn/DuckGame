# CustomProgram

CustomProgram is a 2D game built with MonoGame and .NET 8. The player controls a duck character and tries to collect hearts while avoiding bombs. The game includes a title screen, instruction screen, score tracking, level progression, day and night effects, animated explosion effects, and restart flow.

## Project overview

This project is a simple arcade style game created as a learning project for C# and game development. The gameplay is built around movement, collision detection, sprite animation, and menu flow. The player moves across the screen, collects special items, and must survive obstacles to keep playing.

## Gameplay

The player controls the duck using the left and right arrow keys. Holding Shift makes the character run faster. The goal is to collect hearts for points and special stars for bonus points. Bombs act as hazards and trigger a game over state when they collide with the player. The game also changes between day and night visual states while it is in progress.

## Main features

1. Title screen with start button
2. Instruction screen before gameplay
3. Player movement and animation
4. Collectible hearts and bonus stars
5. Bomb obstacles that cause failure
6. Score and level display
7. Explosion effect on collision
8. Day and night overlay transition
9. Restart option after game over or win

## Controls

Use the following controls while playing:

1. Left Arrow to move left
2. Right Arrow to move right
3. Left Shift or Right Shift to run faster
4. Escape to exit the game
5. F1 to toggle hitbox display

## How to run

Open the solution file in Visual Studio or use the command line from the project folder.

1. Open CustomProgram.sln in Visual Studio
2. Set the startup project if needed
3. Press F5 to run the game

If using the terminal, run the following command inside the project folder:

```bash
dotnet run
```

## Project structure

The main code is organized as follows:

- Program.cs starts the game
- Game1.cs contains the main game loop, scene flow, rendering, and game logic
- Player.cs handles player movement and animation
- Collectible.cs defines collectible items and scoring
- Obstacle.cs defines hazards and movement
- CustomProgram.csproj contains the MonoGame project configuration and package references

## Technologies used

This project uses:

- C#
- .NET 8
- MonoGame
- Microsoft XNA framework components

## Notes

The project is designed as a small game prototype and can be expanded with sound effects, more levels, enemy behavior, or a more advanced UI. The code structure supports simple improvements if you want to extend the game further.
