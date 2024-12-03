namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            Board board = ParseInput(input);

            int max = 0;

            for (int i = 0; i < board.Width; i++)
            {
                max = Math.Max(max, FindEnergizedFromStart(board, i, 0, Direction.Down));
                max = Math.Max(max, FindEnergizedFromStart(board, i, board.Height - 1, Direction.Up));
            }

            for (int i = 0; i < board.Height; i++)
            {
                max = Math.Max(max, FindEnergizedFromStart(board, 0, i, Direction.Right));
                max = Math.Max(max, FindEnergizedFromStart(board, board.Width - 1, i, Direction.Left));
            }

            Console.WriteLine($"Result: {max}");
        }

        static int FindEnergizedFromStart(Board board, int startX, int startY, Direction startDir)
        {
            List<(int, int, Direction)> queue = new List<(int, int, Direction)>
            {
                (startX, startY, startDir)
            };

            while (queue.Count > 0)
            {
                (int x, int y, Direction dir) = queue[0];
                queue.RemoveAt(0);

                Square square = board.Squares[x, y];
                if (square.VisitedLights.Contains(dir))
                {
                    continue;
                }

                square.VisitedLights.Add(dir);

                List<(int, int, Direction)> newLights = new List<(int, int, Direction)>();
                switch (square.Type)
                {
                    case SquareType.Empty:
                        if (dir == Direction.Up)
                        {
                            newLights.Add((x, y - 1, Direction.Up));
                        }
                        else if (dir == Direction.Down)
                        {
                            newLights.Add((x, y + 1, Direction.Down));
                        }
                        else if (dir == Direction.Left)
                        {
                            newLights.Add((x - 1, y, Direction.Left));
                        }
                        else if (dir == Direction.Right)
                        {
                            newLights.Add((x + 1, y, Direction.Right));
                        }
                        else
                        {
                            throw new Exception($"Unhandled dir case '{dir}'");
                        }
                        break;
                    case SquareType.Vertical:
                        if (dir == Direction.Up)
                        {
                            newLights.Add((x, y - 1, Direction.Up));
                        }
                        else if (dir == Direction.Down)
                        {
                            newLights.Add((x, y + 1, Direction.Down));
                        }
                        else if (dir == Direction.Left || dir == Direction.Right)
                        {
                            newLights.Add((x, y - 1, Direction.Up));
                            newLights.Add((x, y + 1, Direction.Down));
                        }
                        else
                        {
                            throw new Exception($"Unhandled dir case '{dir}'");
                        }
                        break;
                    case SquareType.Horizontal:
                        if (dir == Direction.Up || dir == Direction.Down)
                        {
                            newLights.Add((x - 1, y, Direction.Left));
                            newLights.Add((x + 1, y, Direction.Right));
                        }
                        else if (dir == Direction.Left)
                        {
                            newLights.Add((x - 1, y, Direction.Left));
                        }
                        else if (dir == Direction.Right)
                        {
                            newLights.Add((x + 1, y, Direction.Right));
                        }
                        else
                        {
                            throw new Exception($"Unhandled dir case '{dir}'");
                        }
                        break;
                    case SquareType.Backslash:
                        if (dir == Direction.Up)
                        {
                            newLights.Add((x - 1, y, Direction.Left));
                        }
                        else if (dir == Direction.Down)
                        {
                            newLights.Add((x + 1, y, Direction.Right));
                        }
                        else if (dir == Direction.Left)
                        {
                            newLights.Add((x, y - 1, Direction.Up));
                        }
                        else if (dir == Direction.Right)
                        {
                            newLights.Add((x, y + 1, Direction.Down));
                        }
                        else
                        {
                            throw new Exception($"Unhandled dir case '{dir}'");
                        }
                        break;
                    case SquareType.ForwardSlash:
                        if (dir == Direction.Up)
                        {
                            newLights.Add((x + 1, y, Direction.Right));
                        }
                        else if (dir == Direction.Down)
                        {
                            newLights.Add((x - 1, y, Direction.Left));
                        }
                        else if (dir == Direction.Left)
                        {
                            newLights.Add((x, y + 1, Direction.Down));
                        }
                        else if (dir == Direction.Right)
                        {
                            newLights.Add((x, y - 1, Direction.Up));
                        }
                        else
                        {
                            throw new Exception($"Unhandled dir case '{dir}'");
                        }
                        break;
                }

                newLights = newLights.FindAll(x => x.Item1 >= 0 && x.Item1 < board.Width && x.Item2 >= 0 && x.Item2 < board.Height);
                queue.AddRange(newLights);
            }

            int count = 0;
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    if (board.Squares[i, j].VisitedLights.Any())
                    {
                        count++;
                    }
                }
            }

            board.ClearLights();
            return count;
        }

        static Board ParseInput(string input)
        {
            string boardText = input.Trim().Replace("\r", "");
            int width = boardText.IndexOf('\n');
            int height = boardText.Count(x => x == '\n') + 1;

            Board board = new Board(width, height);

            foreach ((int idx, string line) in boardText.Split('\n').Select((txt, idx) => (idx, txt)))
            {
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    SquareType type = c switch
                    {
                        '-' => SquareType.Horizontal,
                        '|' => SquareType.Vertical,
                        '\\' => SquareType.Backslash,
                        '/' => SquareType.ForwardSlash,
                        _ => SquareType.Empty,
                    };

                    board.Squares[i, idx] = new Square(type);
                }
            }

            return board;
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Square[,] Squares;

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            Squares = new Square[width, height];
        }

        public void ClearLights()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Square square = Squares[i, j];
                    square.VisitedLights = new List<Direction>();
                }
            }
        }

        public void Print()
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Square square = Squares[i, j];

                    if (square.VisitedLights.Any())
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    }

                    switch (square.Type)
                    {
                        case SquareType.Empty:
                            Console.Write('.');
                            break;
                        case SquareType.Vertical:
                            Console.Write('|');
                            break;
                        case SquareType.Horizontal:
                            Console.Write('-');
                            break;
                        case SquareType.Backslash:
                            Console.Write('\\');
                            break;
                        case SquareType.ForwardSlash:
                            Console.Write('/');
                            break;
                    }

                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine();
            }
        }
    }

    class Square
    {
        public SquareType Type;
        public List<Direction> VisitedLights;

        public Square(SquareType type)
        {
            Type = type;
            VisitedLights = new List<Direction>();
        }
    }

    enum SquareType
    {
        Empty,
        Vertical,
        Horizontal,
        Backslash,
        ForwardSlash,
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}
