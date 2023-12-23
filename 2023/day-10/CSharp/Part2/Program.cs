namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            Matrix matrix = InputParser.ParseInput(input);
            FindLoopAndLeftRight(matrix);

            matrix.PrintMatrix(false, true);

            int total = 0;
            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height;  j++)
                {
                    if (matrix.Cells[i, j].Type == CellType.Left)
                    {
                        total++;
                    }
                }
            }
        }

        private static void FindLoopAndLeftRight(Matrix matrix)
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
                FindLoop(matrix, startCell);
            }, desiredStackSize);
            newThread.Start();
            newThread.Join();

            newThread = new Thread(() =>
            {
                FindLeftRight(matrix, startCell);
            }, desiredStackSize);
            newThread.Start();
            newThread.Join();
        }

        private static void RecursiveSetType(Matrix matrix, Cell cell, CellType type)
        {
            if (cell.Type != CellType.Unknown)
            {
                return;
            }

            cell.Type = type;

            if (cell.X + 1 < matrix.Width)
            {
                Cell otherCell = matrix.Cells[cell.X + 1, cell.Y];
                if (otherCell.Type == CellType.Unknown)
                {
                    RecursiveSetType(matrix, otherCell, type);
                }
            }

            if (cell.Y + 1 < matrix.Height)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y + 1];
                if (otherCell.Type == CellType.Unknown)
                {
                    RecursiveSetType(matrix, otherCell, type);
                }
            }

            if (cell.X - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X - 1, cell.Y];
                if (otherCell.Type == CellType.Unknown)
                {
                    RecursiveSetType(matrix, otherCell, type);
                }
            }

            if (cell.Y - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y - 1];
                if (otherCell.Type == CellType.Unknown)
                {
                    RecursiveSetType(matrix, otherCell, type);
                }
            }
        }

        private static void FindLoop(Matrix matrix, Cell cell)
        {
            cell.Type = CellType.Loop;

            //if (cell.X == 138 && cell.Y == 77)
            //{

            //}

            List<char> validRightChars = new List<char> { 'S', '-', 'F', 'L' };
            List<char> validLeftChars = new List<char> { 'S', '-', '7', 'J' };
            List<char> validUpChars = new List<char> { 'S', '|', 'J', 'L' };
            List<char> validDownChars = new List<char> { 'S', '|', '7', 'F' };

            if (validRightChars.Contains(cell.Pipe))
            {
                if (cell.X + 1 < matrix.Width)
                {
                    Cell otherCell = matrix.Cells[cell.X + 1, cell.Y];
                    if (otherCell.Type != CellType.Loop)
                    {
                        FindLoop(matrix, otherCell);
                    }
                }
            }

            if (validLeftChars.Contains(cell.Pipe))
            {
                if (cell.X - 1 >= 0)
                {
                    Cell otherCell = matrix.Cells[cell.X - 1, cell.Y];
                    if (otherCell.Type != CellType.Loop)
                    {
                        FindLoop(matrix, otherCell);
                    }
                }
            }

            if (validDownChars.Contains(cell.Pipe))
            {
                if (cell.Y + 1 < matrix.Height)
                {
                    Cell otherCell = matrix.Cells[cell.X, cell.Y + 1];
                    if (otherCell.Type != CellType.Loop)
                    {
                        FindLoop(matrix, otherCell);
                    }
                }
            }

            if (validUpChars.Contains(cell.Pipe))
            {
                if (cell.Y - 1 >= 0)
                {
                    Cell otherCell = matrix.Cells[cell.X, cell.Y - 1];
                    if (otherCell.Type != CellType.Loop)
                    {
                        FindLoop(matrix, otherCell);
                    }
                }
            }
        }

        private static void FindLeftRight(Matrix matrix, Cell cell)
        {
            cell.Visited = true;

            if (cell.X + 1 < matrix.Width)
            {
                Cell otherCell = matrix.Cells[cell.X + 1, cell.Y];
                if (otherCell.Pipe == '-' || otherCell.Pipe == 'J' || otherCell.Pipe == '7')
                {
                    if (!otherCell.Visited)
                    {
                        if (otherCell.Pipe == '-')
                        {
                            if (cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 1], CellType.Left);
                            }

                            if (cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 1], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == 'J')
                        {
                            if (cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 1], CellType.Right);
                            }

                            if (cell.X + 2 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 2, cell.Y], CellType.Right);
                            }

                            if (cell.X + 2 < matrix.Width && cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 1], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == '7')
                        {
                            if (cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 1], CellType.Left);
                            }

                            if (cell.X + 2 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 2, cell.Y], CellType.Left);
                            }

                            if (cell.X + 2 < matrix.Width && cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 1], CellType.Left);
                            }
                        }

                        FindLeftRight(matrix, otherCell);
                    }
                }
            }

            if (cell.X - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X - 1, cell.Y];
                if (otherCell.Pipe == '-' || otherCell.Pipe == 'F' || otherCell.Pipe == 'L')
                {
                    if (!otherCell.Visited)
                    {
                        if (otherCell.Pipe == '-')
                        {
                            if (cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y - 1], CellType.Right);
                            }

                            if (cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y + 1], CellType.Left);
                            }
                        }

                        if (otherCell.Pipe == 'F')
                        {
                            if (cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y - 1], CellType.Right);
                            }

                            if (cell.X - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 2, cell.Y], CellType.Right);
                            }

                            if (cell.X - 2 >= 0 && cell.Y - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 2, cell.Y - 1], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == 'L')
                        {
                            if (cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y + 1], CellType.Left);
                            }

                            if (cell.X - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 2, cell.Y], CellType.Left);
                            }

                            if (cell.X - 2 >= 0 && cell.Y + 1 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 2, cell.Y + 1], CellType.Left);
                            }
                        }

                        FindLeftRight(matrix, otherCell);
                    }
                }
            }

            if (cell.Y + 1 < matrix.Height)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y + 1];
                if (otherCell.Pipe == '|' || otherCell.Pipe == 'L' || otherCell.Pipe == 'J')
                {
                    if (!otherCell.Visited)
                    {
                        if (otherCell.Pipe == '|')
                        {
                            if (cell.X - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y + 1], CellType.Right);
                            }

                            if (cell.X + 1 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 1], CellType.Left);
                            }
                        }

                        if (otherCell.Pipe == 'L')
                        {
                            if (cell.X - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y + 1], CellType.Right);
                            }

                            if (cell.Y + 2 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X, cell.Y + 2], CellType.Right);
                            }

                            if (cell.X - 1 >= 0 && cell.Y + 2 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y + 2], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == 'J')
                        {
                            if (cell.X + 1 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 1], CellType.Left);
                            }

                            if (cell.Y + 2 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X, cell.Y + 2], CellType.Left);
                            }

                            if (cell.X + 1 < matrix.Width && cell.Y + 2 < matrix.Height)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y + 2], CellType.Left);
                            }
                        }

                        FindLeftRight(matrix, otherCell);
                    }
                }
            }

            if (cell.Y - 1 >= 0)
            {
                Cell otherCell = matrix.Cells[cell.X, cell.Y - 1];
                if (otherCell.Pipe == '|' || otherCell.Pipe == '7' || otherCell.Pipe == 'F')
                {
                    if (!otherCell.Visited)
                    {
                        if (otherCell.Pipe == '|')
                        {
                            if (cell.X - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y - 1], CellType.Left);
                            }

                            if (cell.X + 1 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 1], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == '7')
                        {
                            if (cell.X + 1 < matrix.Width)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 1], CellType.Right);
                            }

                            if (cell.Y - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X, cell.Y - 2], CellType.Right);
                            }

                            if (cell.X + 1 < matrix.Width && cell.Y - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X + 1, cell.Y - 2], CellType.Right);
                            }
                        }

                        if (otherCell.Pipe == 'F')
                        {
                            if (cell.X - 1 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y - 1], CellType.Left);
                            }

                            if (cell.Y - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X, cell.Y - 2], CellType.Left);
                            }

                            if (cell.X - 1 >= 0 && cell.Y - 2 >= 0)
                            {
                                RecursiveSetType(matrix, matrix.Cells[cell.X - 1, cell.Y - 2], CellType.Left);
                            }
                        }

                        FindLeftRight(matrix, otherCell);
                    }
                }
            }
        }
    }

    public enum CellType
    {
        Unknown,
        Loop,
        Left,
        Right
    }

    public class Cell
    {
        public char Pipe;
        public CellType Type = CellType.Unknown;
        public bool Visited = false;

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

        public void PrintMatrix(bool onlyShowLoop = false, bool showTypes = false)
        {
            Console.Clear();
            Console.ResetColor();

            for (int j = 0;  j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Cell cell = Cells[i, j];

                    Console.ResetColor();

                    if (onlyShowLoop && cell.Type != CellType.Loop)
                    {
                        Console.Write(" ");
                        continue;
                    }

                    if (showTypes && cell.Type == CellType.Loop)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write(cell.Pipe);
                        Console.ResetColor();
                        continue;
                    }

                    if (showTypes && cell.Type == CellType.Left)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(cell.Pipe);
                        Console.ResetColor();
                        continue;
                    }

                    if (showTypes && cell.Type == CellType.Right)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(cell.Pipe);
                        Console.ResetColor();
                        continue;
                    }

                    Console.Write(cell.Pipe);
                }
                Console.WriteLine();
            }
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
