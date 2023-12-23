namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            Matrix matrix = InputParser.ParseInput(input);
            int result = FindMaxDepth(matrix);

        }

        private static int FindMaxDepth(Matrix matrix)
        {
            Cell startCell = null;

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    if (matrix.Cells[i, j].Pipe == 'S')
                    {
                        startCell = matrix.Cells[i, j];
                        break;
                    }
                }
            }

            int desiredStackSize = 2 * 1024 * 1024 * 100; // 2 MB
            Thread newThread = new Thread(() =>
            {
                FindMaxDepthHelper(matrix, startCell, 0);
            }, desiredStackSize);
            newThread.Start();
            newThread.Join();

            //FindMaxDepthHelper(matrix, startCell, 0);

            int largestDepth = 0;
            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    Cell cell = matrix.Cells[i, j];

                    if (cell.Depth == int.MaxValue)
                    {
                        continue;
                    }

                    largestDepth = Math.Max(largestDepth, cell.Depth);
                }
            }

            return largestDepth;
        }

        private static void FindMaxDepthHelper(Matrix matrix, Cell cell, int currentDepth)
        {
            cell.Depth = currentDepth;

            if (cell.X + 1 < matrix.Width)
            {
                Cell otherCell = matrix.Cells[cell.X + 1, cell.Y];
                if (otherCell.Pipe == '-' || otherCell.Pipe == 'J' || otherCell.Pipe == '7')
                {
                    if (cell.Depth + 1 < otherCell.Depth)
                    {
                        FindMaxDepthHelper(matrix, otherCell, currentDepth + 1);
                    }
                }
            }

            if (cell.X - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X - 1, cell.Y];
                if (otherCell.Pipe == '-' || otherCell.Pipe == 'F' || otherCell.Pipe == 'L')
                {
                    if (cell.Depth + 1 < otherCell.Depth)
                    {
                        FindMaxDepthHelper(matrix, otherCell, currentDepth + 1);
                    }
                }
            }

            if (cell.Y + 1 < matrix.Height)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y + 1];
                if (otherCell.Pipe == '|' || otherCell.Pipe == 'L' || otherCell.Pipe == 'J')
                {
                    if (cell.Depth + 1 < otherCell.Depth)
                    {
                        FindMaxDepthHelper(matrix, otherCell, currentDepth + 1);
                    }
                }
            }

            if (cell.Y - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y - 1];
                if (otherCell.Pipe == '|' || otherCell.Pipe == '7' || otherCell.Pipe == 'F')
                {
                    if (cell.Depth + 1 < otherCell.Depth)
                    {
                        FindMaxDepthHelper(matrix, otherCell, currentDepth + 1);
                    }
                }
            }


        }
    }

    public class Cell
    {
        public char Pipe;
        public int Depth = int.MaxValue;

        public int X;
        public int Y;

        public Cell(char pipe, int x, int y)
        {
            Pipe = pipe;
            X = x;
            Y = y;
        }
    }

    public class Matrix
    {
        public int Width;
        public int Height;

        public Cell[,] Cells;

        public Matrix(int width, int height, Cell[,] cells)
        {
            Width = width;
            Height = height;
            Cells = cells;
        }
    }

    public static class InputParser
    {
        public static Matrix ParseInput(string input)
        {
            string[] lines = input.Split("\n").Where(x => x != "").ToArray();
            int width = lines.First().Length;
            int height = lines.Count();

            Cell[,] cells = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j] = new Cell(lines[j][i], i, j);
                }
            }

            return new Matrix(width, height, cells);
        }
    }
}
