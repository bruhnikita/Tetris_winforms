namespace Tetris2
{
    public partial class Form1 : Form
    {
        private const int Rows = 20;
        private const int Columns = 10;
        private const int BlockSize = 30;
        private System.Windows.Forms.Timer gameTimer;
        private int[,] grid = new int[Rows, Columns];
        Tetromino currentTetromino;
        Point tetrominoPosition = new Point(3, 0);
        private int score = 0;
        private int level = 1;

        public Form1()
        {
            this.Text = "Тетрис";
            this.ClientSize = new Size(Columns * BlockSize, Rows * BlockSize);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 500;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            GenerateNewTetromino();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (CanMove(currentTetromino, 0, 1))
            {
                tetrominoPosition.Y++;
            }
            else
            {
                LockTetromino();
                ClearLines();
                GenerateNewTetromino();
            }

            Invalidate();
        }

        private bool CanMove(Tetromino tetromino, int dx, int dy)
        {
            foreach (var block in tetromino.Shape)
            {
                int newX = block.X + tetrominoPosition.X + dx;
                int newY = block.Y + tetrominoPosition.Y + dy;

                if (newX < 0 || newX >= Columns || newY >= Rows || grid[newY, newX] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void LockTetromino()
        {
            foreach (var block in currentTetromino.Shape)
            {
                grid[block.Y + tetrominoPosition.Y, block.X + tetrominoPosition.X] = 1;
            }
        }

        private void GenerateNewTetromino()
        {
            Tetromino[] tetrominoes = {
                new Tetromino(new Point[] { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1) }), // I
                new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) }), // O
                new Tetromino(new Point[] { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(2, 0) }), // L
                new Tetromino(new Point[] { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(0, 0) }), // J
                new Tetromino(new Point[] { new Point(1, 0), new Point(2, 0), new Point(0, 1), new Point(1, 1) }), // S
                new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1) }), // Z
                new Tetromino(new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1) }), // T
            };
            Random rand = new Random();
            currentTetromino = tetrominoes[rand.Next(tetrominoes.Length)];
            tetrominoPosition = new Point(3, 0);

            if (!CanMove(currentTetromino, 0, 0))
            {
                GameOver();
            }
        }

        private void ClearLines()
        {
            int linesCleared = 0;
            for (int r = Rows - 1; r >= 0; r--)
            {
                bool isLineFull = true;

                for (int c = 0; c < Columns; c++)
                {
                    if (grid[r, c] == 0)
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    linesCleared++;
                    for (int rr = r; rr > 0; rr--)
                    {
                        for (int c = 0; c < Columns; c++)
                        {
                            grid[rr, c] = grid[rr - 1, c];
                        }
                    }

                    for (int c = 0; c < Columns; c++)
                    {
                        grid[0, c] = 0;
                    }

                    r++;
                }
            }

            score += linesCleared * 100;
            if (linesCleared > 0)
            {
                level = 1 + score / 1000;
                gameTimer.Interval = Math.Max(100, 500 - (level - 1) * 50);
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Игра окончена! Ваш счет: {score}", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (grid[r, c] != 0)
                    {
                        g.FillRectangle(Brushes.Blue, c * BlockSize, r * BlockSize, BlockSize, BlockSize);
                        g.DrawRectangle(Pens.Black, c * BlockSize, r * BlockSize, BlockSize, BlockSize);
                    }
                }
            }

            foreach (var block in currentTetromino.Shape)
            {
                int x = (block.X + tetrominoPosition.X) * BlockSize;
                int y = (block.Y + tetrominoPosition.Y) * BlockSize;
                g.FillRectangle(Brushes.Red, x, y, BlockSize, BlockSize);
                g.DrawRectangle(Pens.Black, x, y, BlockSize, BlockSize);
            }

            g.DrawString($"Score: {score}", new Font("Arial", 16), Brushes.Black, new Point(10, 10));
            g.DrawString($"Level: {level}", new Font("Arial", 16), Brushes.Black, new Point(10, 40));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left && CanMove(currentTetromino, -1, 0))
            {
                tetrominoPosition.X--;
            }
            else if (keyData == Keys.Right && CanMove(currentTetromino, 1, 0))
            {
                tetrominoPosition.X++;
            }
            else if (keyData == Keys.Down && CanMove(currentTetromino, 0, 1))
            {
                tetrominoPosition.Y++;
            }
            else if (keyData == Keys.Up)
            {
                currentTetromino.Rotate(tetrominoPosition, grid);
            }

            Invalidate();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
