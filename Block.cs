using System;

public class Block
{
    public int Rows { get; }
    public int Cols { get; }
    public int BlockWidth { get; }
    public int StartX { get; }
    public int StartY { get; }
    public bool[,] Alive;

    public Block(Map map, int rows = 5, int cols = 12, int blockWidth = 4, int startY = 3)
    {
        Rows = rows;
        Cols = cols;
        BlockWidth = blockWidth;
        StartY = startY;
        StartX = (map.Width - cols * (blockWidth + 1)) / 2;
        Alive = new bool[rows, cols];
    }

    public void Init()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Alive[r, c] = true;
    }

    public bool AnyAlive()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (Alive[r, c]) return true;
        return false;
    }
}
