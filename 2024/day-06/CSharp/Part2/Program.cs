using System.Transactions;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = ParseInput(input);

            int result = board.CountPossibleLoops();
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
                        board.GuardStartX = j;
                        board.GuardStartY = i;
                        board.GuardX = j;
                        board.GuardY = i;
                        board.Spaces[j, i].VisitedDirections.Add(Direction.North);
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

        public int GuardStartX = -1;
        public int GuardStartY = -1;
        public int GuardX = -1;
        public int GuardY = -1;
        public Direction GuardFacing = Direction.North;

        private IEnumerable<(int x, int y, Direction dir, Space space)> IterWalk()
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
                        yield return (GuardX, GuardY, GuardFacing, Spaces[GuardX, GuardY]);

                        GuardX = newX;
                        GuardY = newY;
                        break;
                    case SpaceType.Obstacle:
                        GuardFacing = GetRightDirection(GuardFacing);
                        break;
                }

                Space guardSpace = Spaces[GuardX, GuardY];
                guardSpace.VisitedDirections.Add(GuardFacing);
            }

            yield break;
        }

        public int CountPossibleLoops()
        {
            int count = 0;

            foreach ((int x, int y, Direction dir, Space space) in IterWalk())
            {
                Direction rightDirection = GetRightDirection(dir);
                List<Space> emptySpaces = GetEmptySpacesToTheRight(x, y, dir);

                if (emptySpaces.Any(x => x.VisitedDirections.Contains(rightDirection)))
                {
                    if (x + 1 == GuardStartX && y + 1 == GuardStartY)
                    {
                        continue;
                    }

                    count++;
                }
            }

            return count;
        }

        private List<Space> GetEmptySpacesToTheRight(int x, int y, Direction dir)
        {
            List<Space> result = new List<Space>();

            int newX = x;
            int newY = y;
            while (true)
            {
                switch (dir)
                {
                    case Direction.North:
                        newX++;
                        break;
                    case Direction.South:
                        newX--;
                        break;
                    case Direction.East:
                        newY++;
                        break;
                    case Direction.West:
                        newY--;
                        break;
                    default:
                        throw new Exception($"Unhandled direction '{dir}'.");
                }

                if (newX < 0 || newY < 0 || newX >= Width || newY >= Height)
                {
                    break;
                }

                Space space = Spaces[newX, newY];

                if (space.Type == SpaceType.Empty)
                {
                    result.Add(space);
                }
                else
                {
                    break;
                }
            }

            return result;
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

        private Direction GetRightDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return Direction.East;
                case Direction.South:
                    return Direction.West;
                case Direction.East:
                    return Direction.South;
                case Direction.West:
                    return Direction.North;
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
                        switch (GuardFacing)
                        {
                            case Direction.North:
                                Console.Write("^");
                                break;
                            case Direction.South:
                                Console.Write("v");
                                break;
                            case Direction.East:
                                Console.Write(">");
                                break;
                            case Direction.West:
                                Console.Write("<");
                                break;
                        }
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
        public List<Direction> VisitedDirections = new List<Direction>();
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
