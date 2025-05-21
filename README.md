# Snake Game

Relive the fun of the classic arcade game with this Snake implementation, built with C# and WPF (Windows Presentation Foundation)! Guide your ever-growing snake to gobble up food while avoiding walls and your own tail.

## Table of Contents

- [Features](#features)
- [Screenshots](#screenshots)
- [How to Play](#how-to-play)
- [Building from Source](#building-from-source)
- [AMath Library](#amath-library)
- [Known Issues](#known-issues)
- [License](#license)

## Features

- Classic Snake gameplay mechanics.
- Customizable themes (loaded from `Data/Themes.json`).
- Persistent high score tracking (using `Data/Scores.bin`).
- Adjustable game settings (e.g., field size, snake speed).
- AI-controlled bot opponent option.
- Support for multiple snakes on the game field.

## Screenshots

![Gameplay screenshot showing the snake, food, and score.](https://user-images.githubusercontent.com/49288795/116287331-54403780-a7a1-11eb-9d7f-1e270c55d1b9.png)
![Game settings menu showing options for field size, speed, and themes.](https://user-images.githubusercontent.com/49288795/116287351-586c5500-a7a1-11eb-878f-8ff61c8381a3.png)

## How to Play

### Controls

- **Arrow Keys (Up, Down, Left, Right):** Control the snake's movement.
- **WASD Keys:** Alternative controls for snake movement.

### Objective

- The primary goal is to guide the snake to eat food pellets that appear on the game field.
- Each pellet consumed makes the snake grow longer.
- Players must avoid colliding with the game boundaries (walls) and the snake's own tail.
- The game ends if the snake collides with a wall or its own tail.

## Building from Source

### Project Type

This is a C# WPF (Windows Presentation Foundation) application.

### Prerequisites

- **.NET 5.0 SDK:** You will need the .NET 5.0 SDK (or a later compatible version) installed to build this project. You can download it from the official .NET website.

### Build Steps

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd <repository-directory>
   ```

2. **Build the solution:**
   Open a terminal or command prompt in the root directory of the cloned repository (where `SnakeGame.sln` is located) and run the following command:
   ```bash
   dotnet build SnakeGame.sln
   ```

3. **Run the application:**
   After a successful build, the main executable can typically be found in:
   `SnakeGame/bin/Debug/net5.0-windows/SnakeGame.exe`
   (The exact path might vary slightly based on your build configuration, e.g., `Release` instead of `Debug`).

## AMath Library

AMath is a small, custom utility library specifically developed for use within the Snake Game. It provides fundamental data types and operations that are essential for the game's graphics rendering and core logic.

### Key Components:

- **`Point`**: A structure used to represent 2D coordinates on the game field (e.g., snake position, food location).
- **`Vector`**: A class that handles 2D vector operations. This is crucial for calculating snake movement directions and other spatial logic.
- **`Color`**: A class for representing and manipulating colors. This is utilized by the game's theming system to customize the appearance of game elements.

## Known Issues

- **Antivirus Flagging:** Some antivirus programs may flag the game executable as potentially unwanted or malicious. The specific cause for this has not been definitively identified. It is believed to be a false positive, possibly due to the way the executable is packed or due to its interaction with system resources (like saving high scores).
  - You can check the executable against multiple antivirus engines using services like VirusTotal. Here is a link to a scan of a previous version of the executable: [VirusTotal Scan Results](https://www.virustotal.com/gui/file/c68b355ada5db463e5ceb000eec49ddddfccaf1cd9771b0465385ef09409374e/detection). *Please note that this link points to a specific version and the current version might have a different hash.*

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
