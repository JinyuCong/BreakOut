# BreakOut

A classic Breakout (brick breaker) game built as a C# console application with .NET 8.0.

## Controls

- `←` / `A` — Move paddle left
- `→` / `D` — Move paddle right
- `ESC` — Quit game

## Project Structure

```
├── Program.cs      # Entry point
├── Game.cs         # Main game controller
├── Ball.cs         # Ball physics & collision detection
├── Paddle.cs       # Player-controlled paddle
├── Block.cs        # Brick grid management
├── Map.cs          # Game arena dimensions
└── BreakOut.csproj # Project configuration
```

## Classes & Key Functions

### Program

| Function | Description |
|----------|-------------|
| `Main()` | Entry point. Creates a `Game` instance and calls `Run()` to start the game |

### Game — Main Game Controller

Manages game state, input handling, and the render loop.

| Function | Description |
|----------|-------------|
| `Run()` | Main game loop. Initializes the console window and runs input handling → ball update → rendering at 20ms intervals |
| `ShowStartScreen()` | Displays the welcome screen with controls and waits for a key press to begin |
| `HandleInput()` | Reads keyboard input to move the paddle or exit via ESC |
| `Render()` | Renders the full game frame: status bar (score, lives), borders, blocks, paddle, and ball. Uses double buffering (char + color arrays) to reduce flicker |
| `ShowEndScreen()` | Displays win/loss message and the final score |
| `CenterText()` | Helper to center text horizontally on a given row |

### Ball — Physics & Collision

Manages ball position, velocity, and all collision logic. Uses double-precision floats for smooth movement.

| Function | Description |
|----------|-------------|
| `Ball()` | Constructor. Places the ball above the paddle center with an initial velocity of (1, -1) |
| `Reset()` | Resets the ball position above the paddle and restores default velocity after a life is lost |
| `Update()` | Core physics update. Processes in priority order: wall bounce → bottom fall (lose life / game over) → paddle collision (with spin based on hit position) → block collision → win check |
| `CheckBlockCollision()` | Iterates through all blocks to detect a hit. On collision, destroys the block, bounces the ball, and awards points |

### Paddle — Player Control

Manages paddle position and movement boundaries.

| Function | Description |
|----------|-------------|
| `Paddle()` | Constructor. Sets paddle width (default 8), positions it near the bottom of the arena, centered horizontally |
| `MoveLeft()` | Moves the paddle 3 units left, clamped to the left border |
| `MoveRight()` | Moves the paddle 3 units right, clamped to the right border |

### Block — Brick Grid

Manages the state of the destructible block grid.

| Function | Description |
|----------|-------------|
| `Block()` | Constructor. Configures the grid (default 5 rows x 12 columns, each block 4 chars wide), centered in the arena |
| `Init()` | Sets all blocks to the alive state |
| `AnyAlive()` | Returns whether any blocks remain, used for the win condition |

### Map — Arena Dimensions

| Function | Description |
|----------|-------------|
| `Map()` | Constructor. Defines the game area size (default 60 wide x 30 tall) |

## Game Mechanics

- **Lives**: Start with 3. Lose one when the ball falls below the paddle.
- **Scoring**: Higher rows are worth more points. Top row = 50, bottom row = 10 (formula: `10 * (totalRows - rowIndex)`).
- **Paddle Spin**: The ball's horizontal velocity changes based on where it hits the paddle — hitting the left side sends it left, the right side sends it right.
- **Block Colors**: Rows are colored top to bottom: Red, Yellow, Green, Cyan, Magenta.

## Run

```bash
dotnet run
```
