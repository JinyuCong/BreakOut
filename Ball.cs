using System;

public class Ball
{
    public double X { get; set; }
    public double Y { get; set; }
    public double DX { get; set; }
    public double DY { get; set; }

    public Ball(Map map, Paddle paddle)
    {
        X = map.Width / 2;
        Y = paddle.Y - 1;
        DX = 1;
        DY = -1;
    }

    public void Reset(Paddle paddle)
    {
        X = paddle.X + paddle.Width / 2;
        Y = paddle.Y - 1;
        DX = 1;
        DY = -1;
    }

    public int RoundX => (int)Math.Round(X);
    public int RoundY => (int)Math.Round(Y);

    public void Update(Map map, Paddle paddle, Block block, Game game)
    {
        double newX = X + DX;
        double newY = Y + DY;

        // 左右墙壁反弹
        if (newX <= 0 || newX >= map.Width - 1)
        {
            DX = -DX;
            newX = X + DX;
        }

        // 上墙壁反弹
        if (newY <= 0)
        {
            DY = -DY;
            newY = Y + DY;
        }

        // 球掉出底部
        if (newY >= map.Height - 1)
        {
            game.Lives--;
            if (game.Lives <= 0)
            {
                game.GameOver = true;
                return;
            }
            Reset(paddle);
            return;
        }

        // 挡板碰撞
        int bxi = (int)Math.Round(newX);
        int byi = (int)Math.Round(newY);
        if (byi == paddle.Y && bxi >= paddle.X && bxi < paddle.X + paddle.Width)
        {
            DY = -Math.Abs(DY);
            double hitPos = (newX - paddle.X) / paddle.Width;
            DX = (hitPos - 0.5) * 2.5;
            if (Math.Abs(DX) < 0.3) DX = DX >= 0 ? 0.3 : -0.3;
            newY = paddle.Y - 1;
        }

        // 方块碰撞
        CheckBlockCollision(bxi, byi, block, game);

        X = newX;
        Y = newY;

        // 检查是否全部消除
        if (!block.AnyAlive()) game.GameWon = true;
    }

    private void CheckBlockCollision(int bx, int by, Block block, Game game)
    {
        for (int r = 0; r < block.Rows; r++)
        {
            for (int c = 0; c < block.Cols; c++)
            {
                if (!block.Alive[r, c]) continue;

                int bkx = block.StartX + c * (block.BlockWidth + 1);
                int bky = block.StartY + r;

                if (by == bky && bx >= bkx && bx < bkx + block.BlockWidth)
                {
                    block.Alive[r, c] = false;
                    DY = -DY;
                    game.Score += 10 * (block.Rows - r);
                    return;
                }
            }
        }
    }
}
