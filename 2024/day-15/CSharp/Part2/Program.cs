namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            (Board board, List<Direction> directions) = Board.ParseInput(input);

            foreach (Direction direction in directions)
            {
                board.MoveRobotInDirection(direction);
            }

            board.Print();
            int result = board.CountBoxCoordinates();
            Console.WriteLine($"Result: {result}");
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;

        public (int x, int y) RobotPos;

        private Board(int width, int height, Tile[,] tiles, (int, int) robotPos)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
            RobotPos = robotPos;
        }

        public int CountBoxCoordinates()
        {
            int count = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Tiles[x, y].Type == TileType.Box)
                    {
                        count += y * 100 + x;
                    }
                }
            }

            return count;
        }

        public void MoveRobotInDirection(Direction dir)
        {
            (int offsetx, int offsety) = GetDirectionOffsets(dir);

            int currentx = RobotPos.x;
            int currenty = RobotPos.y;

            while (true)
            {
                currentx += offsetx;
                currenty += offsety;

                Tile tile = Tiles[currentx, currenty];

                if (tile.Type == TileType.Empty)
                {
                    break;
                }

                if (tile.Type == TileType.Wall)
                {
                    return;
                }
            }

            while (true)
            {
                currentx -= offsetx;
                currenty -= offsety;

                if (currentx == RobotPos.x && currenty == RobotPos.y)
                {
                    break;
                }

                Tiles[currentx + offsetx, currenty + offsety].Type = TileType.Box;
                Tiles[currentx, currenty].Type = TileType.Empty;
            }

            RobotPos = (RobotPos.x + offsetx, RobotPos.y + offsety);
        }

        private (int, int) GetDirectionOffsets(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return (0, -1);
                case Direction.Down:
                    return (0, 1);
                case Direction.Right:
                    return (1, 0);
                case Direction.Left:
                    return (-1, 0);
                default:
                    throw new Exception($"Unhandled direction enum '{dir}'.");
            }
        }

        public static (Board, List<Direction>) ParseInput(string input)
        {
            string[] split = input.Split("\n\n");
            split[1] = split[1].Replace("\n", "");

            string[] lines = split[0].Split("\n");
            int width = lines[0].Length;
            int height = lines.Length;

            Tile[,] tiles = new Tile[width, height];

            (int, int)? robotPos = null;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = lines[y][x];
                    TileType type = c switch
                    {
                        '.' => TileType.Empty,
                        '@' => TileType.Empty,
                        '#' => TileType.Wall,
                        'O' => TileType.Box,
                        _ => throw new Exception($"Does not recognize '{c}' as a valid board character.")
                    };


                    tiles[x, y] = new Tile(type);
                    if (c == '@')
                    {
                        robotPos = (x, y);
                    }
                }
            }

            if (!robotPos.HasValue)
            {
                throw new Exception("The robot position is not set");
            }

            Board board = new Board(width, height, tiles, robotPos.Value);

            List<Direction> directions = [];
            foreach (char c in split[1])
            {
                switch (c)
                {
                    case '^':
                        directions.Add(Direction.Up);
                        break;
                    case 'v':
                        directions.Add(Direction.Down);
                        break;
                    case '>':
                        directions.Add(Direction.Right);
                        break;
                    case '<':
                        directions.Add(Direction.Left);
                        break;
                    default:
                        throw new Exception($"Does not recognize '{c}' as a valid direction character.");
                }
            }

            return (board, directions);
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile = Tiles[x, y];
                    TileType type = tile.Type;
                    char tileTypeChar = type switch
                    {
                        TileType.Empty => '.',
                        TileType.Wall => '#',
                        TileType.Box => 'O',
                        _ => throw new Exception($"Unhandled tile type enum '{tile.Type}'.")
                    };

                    if ((x, y) == RobotPos)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }

                    Console.Write(tileTypeChar);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
        }
    }

    class Tile(TileType type)
    {
        public TileType Type = type;
    }

    enum TileType
    {
        Empty,
        Wall,
        Box,
    }

    enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }
}
