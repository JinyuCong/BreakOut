public class Paddle
{
    public int X { get; set; }
    public int Y { get; }
    public int Width { get; }

    public Paddle(Map map, int width = 8)
    {
        Width = width;
        Y = map.Height - 2;
        X = map.Width / 2 - width / 2;
    }

    public void MoveLeft()
    {
        X -= 3;
        if (X < 1) X = 1;
    }

    public void MoveRight(int mapWidth)
    {
        X += 3;
        if (X + Width > mapWidth - 1) X = mapWidth - 1 - Width;
    }
}
