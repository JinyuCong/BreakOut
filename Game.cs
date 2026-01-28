using System;
using System.Threading;

public class Game
{
    public Map Map;
    public Paddle Paddle;
    public Block Block;
    public Ball Ball;

    public int Score { get; set; }
    public int Lives { get; set; } = 3;
    public bool GameOver { get; set; }
    public bool GameWon { get; set; }

    private char[,] buffer;
    private ConsoleColor[,] colorBuffer;

    private static readonly ConsoleColor[] RowColors = {
        ConsoleColor.Red,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.Cyan,
        ConsoleColor.Magenta
    };

    public Game()
    {
        Map = new Map();
        Paddle = new Paddle(Map);
        Block = new Block(Map);
        Ball = new Ball(Map, Paddle);
        buffer = new char[Map.Height, Map.Width];
        colorBuffer = new ConsoleColor[Map.Height, Map.Width];
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Title = "打方块 - Breakout Game";

        try
        {
            Console.SetWindowSize(Math.Max(Map.Width + 2, Console.WindowWidth), Math.Max(Map.Height + 4, Console.WindowHeight));
            Console.SetBufferSize(Math.Max(Map.Width + 2, Console.BufferWidth), Math.Max(Map.Height + 4, Console.BufferHeight));
        }
        catch { }

        Block.Init();
        ShowStartScreen();

        while (!GameOver && !GameWon)
        {
            HandleInput();
            Ball.Update(Map, Paddle, Block, this);
            Render();
            Thread.Sleep(20);
        }

        ShowEndScreen();
        Console.CursorVisible = true;
    }

    private void ShowStartScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        int cy = Map.Height / 2 - 3;
        CenterText(cy,     "╔══════════════════════════╗");
        CenterText(cy + 1, "║     打 方 块 游 戏       ║");
        CenterText(cy + 2, "║                          ║");
        CenterText(cy + 3, "║   ← → 移动挡板           ║");
        CenterText(cy + 4, "║   按任意键开始...         ║");
        CenterText(cy + 5, "╚══════════════════════════╝");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }

    private void CenterText(int y, string text)
    {
        int x = (Map.Width - text.Length) / 2;
        if (x < 0) x = 0;
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    private void HandleInput()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow || key == ConsoleKey.A)
                Paddle.MoveLeft();
            else if (key == ConsoleKey.RightArrow || key == ConsoleKey.D)
                Paddle.MoveRight(Map.Width);
            else if (key == ConsoleKey.Escape)
                GameOver = true;

            while (Console.KeyAvailable) Console.ReadKey(true);
        }
    }

    private void Render()
    {
        Console.SetCursorPosition(0, 0);

        // 状态栏
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        string status = $" 分数: {Score}    生命: {new string('♥', Lives)}    ESC退出 ";
        Console.Write(status.PadRight(Map.Width));
        Console.ResetColor();

        // 清空缓冲区
        for (int y = 0; y < Map.Height; y++)
            for (int x = 0; x < Map.Width; x++)
            {
                buffer[y, x] = ' ';
                colorBuffer[y, x] = ConsoleColor.Gray;
            }

        // 边框
        for (int y = 0; y < Map.Height; y++)
        {
            buffer[y, 0] = '│';
            buffer[y, Map.Width - 1] = '│';
            colorBuffer[y, 0] = ConsoleColor.DarkGray;
            colorBuffer[y, Map.Width - 1] = ConsoleColor.DarkGray;
        }
        for (int x = 0; x < Map.Width; x++)
        {
            buffer[0, x] = '─';
            colorBuffer[0, x] = ConsoleColor.DarkGray;
        }
        buffer[0, 0] = '┌';
        buffer[0, Map.Width - 1] = '┐';

        // 方块
        for (int r = 0; r < Block.Rows; r++)
            for (int c = 0; c < Block.Cols; c++)
            {
                if (!Block.Alive[r, c]) continue;
                int bx = Block.StartX + c * (Block.BlockWidth + 1);
                int by = Block.StartY + r;
                for (int i = 0; i < Block.BlockWidth && bx + i < Map.Width - 1; i++)
                {
                    buffer[by, bx + i] = '█';
                    colorBuffer[by, bx + i] = RowColors[r % RowColors.Length];
                }
            }

        // 挡板
        for (int i = 0; i < Paddle.Width; i++)
        {
            int px = Paddle.X + i;
            if (px > 0 && px < Map.Width - 1)
            {
                buffer[Paddle.Y, px] = '▀';
                colorBuffer[Paddle.Y, px] = ConsoleColor.White;
            }
        }

        // 球
        int bxi = Ball.RoundX;
        int byi = Ball.RoundY;
        if (bxi >= 0 && bxi < Map.Width && byi >= 0 && byi < Map.Height)
        {
            buffer[byi, bxi] = 'O';
            colorBuffer[byi, bxi] = ConsoleColor.Yellow;
        }

        // 逐行输出
        for (int y = 0; y < Map.Height; y++)
        {
            Console.SetCursorPosition(0, y + 1);
            ConsoleColor currentColor = ConsoleColor.Gray;
            Console.ForegroundColor = currentColor;

            for (int x = 0; x < Map.Width; x++)
            {
                if (colorBuffer[y, x] != currentColor)
                {
                    currentColor = colorBuffer[y, x];
                    Console.ForegroundColor = currentColor;
                }
                Console.Write(buffer[y, x]);
            }
        }
        Console.ResetColor();
    }

    private void ShowEndScreen()
    {
        Console.SetCursorPosition(0, Map.Height / 2);
        if (GameWon)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            CenterText(Map.Height / 2, "★★★ 恭喜你赢了！ ★★★");
            CenterText(Map.Height / 2 + 1, $"最终得分: {Score}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterText(Map.Height / 2, "游戏结束！");
            CenterText(Map.Height / 2 + 1, $"最终得分: {Score}");
        }
        Console.ResetColor();
        CenterText(Map.Height / 2 + 3, "按任意键退出...");
        Console.ReadKey(true);
    }
}
