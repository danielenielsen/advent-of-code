namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = ParseInput(input);
            board.Walk();

            int result = board.CountVisited();
            Console.WriteLine($"Result: {result}");
        }

        static Board ParseInput(string input)
        {
            List<string> lines = input.Split("\n").ToList();

            int width = lines[0].Count();
            int height = lines.Count;

            Board board = new Board(width, height);

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Count(); j++)
                {
                    (SpaceType type, bool guardStart) = lines[i][j] switch
                    {
                        '.' => (SpaceType.Empty, false),
                        '^' => (SpaceType.Empty, true),
                        '#' => (SpaceType.Obstacle, false),
                        _ => throw new Exception($"The character {lines[i][j]} is not recognized.")
                    };

                    board.Spaces[j, i] = new Space(type);

                    if (guardStart)
                    {
                        board.GuardX = j;
                        board.GuardY = i;
                        board.Spaces[j, i].Visited = true;
                    }
                }
            }

            if (board.GuardX == -1 || board.GuardY == -1)
            {
                throw new Exception($"Guard starting position not correctly set, it was ({board.GuardX},{board.GuardY}).");
            }

            return board;
        }
    }

    class Board(int width, int height)
    {
        public int Width = width;
        public int Height = height;
        public Space[,] Spaces = new Space[width, height];

        public int GuardX = -1;
        public int GuardY = -1;
        public Direction GuardFacing = Direction.North;

        public void Walk()
        {
            while (true)
            {
                (int newX, int newY) = GetCoordsBasedOnDirection();

                if (newX < 0 || newX >= Width || newY < 0 || newY >= Height)
                {
                    break;
                }

                Space newSpace = Spaces[newX, newY];

                switch (newSpace.Type)
                {
                    case SpaceType.Empty:
                        newSpace.Visited = true;
                        GuardX = newX;
                        GuardY = newY;
                        break;
                    case SpaceType.Obstacle:
                        TurnGuard();
                        break;
                }
            }
        }

        public int CountVisited()
        {
            int count = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Spaces[j, i].Visited)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private (int x, int y) GetCoordsBasedOnDirection()
        {
            switch (GuardFacing)
            {
                case Direction.North:
                    return (GuardX, GuardY - 1);
                case Direction.South:
                    return (GuardX, GuardY + 1);
                case Direction.East:
                    return (GuardX + 1, GuardY);
                case Direction.West:
                    return (GuardX - 1, GuardY);
                default:
                    throw new Exception($"Unhandled direction '{GuardFacing}'.");
            }
        }

        private void TurnGuard()
        {
            switch (GuardFacing)
            {
                case Direction.North:
                    GuardFacing = Direction.East;
                    break;
                case Direction.South:
                    GuardFacing = Direction.West;
                    break;
                case Direction.East:
                    GuardFacing = Direction.South;
                    break;
                case Direction.West:
                    GuardFacing = Direction.North;
                    break;
                default:
                    throw new Exception($"Unhandled direction '{GuardFacing}'.");
            }
        }

        public void Print()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (GuardX == j && GuardY == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("^");
                        Console.BackgroundColor = ConsoleColor.Black;
                        continue;
                    }

                    switch (Spaces[j, i].Type)
                    {
                        case SpaceType.Empty:
                            Console.Write(".");
                            break;
                        case SpaceType.Obstacle:
                            Console.Write("#");
                            break;
                    }
                }

                Console.WriteLine();
            }
        }
    }

    class Space(SpaceType type)
    {
        public SpaceType Type = type;
        public bool Visited = false;
    }

    enum SpaceType
    {
        Empty,
        Obstacle
    }

    enum Direction
    {
        North,
        South,
        East,
        West,
    }
}
