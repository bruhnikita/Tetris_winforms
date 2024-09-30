using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    public class Tetromino
    {
        public Point[] Shape {  get; set; }
        public int Rotation { get; set; }

        public Tetromino(Point[] shape)
        {
            Shape = shape;
            Rotation = 0;
        }

        public void Rotate(Point gridPosition, int[,] grid)
        {
            Point[] rotatedShape = new Point[Shape.Length];

            for (int i = 0; i < Shape.Length; i++)
            {
                int newX = -Shape[i].Y;
                int newY = Shape[i].X;
                rotatedShape[i] = new Point(newX, newY);
            }

            if (CanRotate(rotatedShape, gridPosition, grid))
            {
                Shape = rotatedShape;
            }
        }

        private bool CanRotate(Point[] rotatedShape, Point gridPosition, int[,] grid)
        {
            foreach (var block in rotatedShape)
            {
                int newX = block.X + gridPosition.X;
                int newY = block.Y + gridPosition.Y;

                if (newX < 0 || newX >= grid.GetLength(1) || newY < 0 || newY >= grid.GetLength(0))
                {
                    return false;
                }

                if (grid[newY, newX] != 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
