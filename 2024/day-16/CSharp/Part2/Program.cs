namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);

            int result = board.CountTilesOnShortestPath();
            board.Print();
            Console.WriteLine($"Tile count on shortest path: {result}");
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;

        public (int x, int y) Start;
        public (int x, int y) End;
        public Direction EndDirection;

        private Board(int width, int height, Tile[,] tiles, (int, int) start, (int, int) end)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
            Start = start;
            End = end;
        }

        public int CountTilesOnShortestPath()
        {
            PopulateDistances();

            List<(int x, int y, Tile tile, Direction dir)> current = [(End.x, End.y, Tiles[End.x, End.y], EndDirection)];


            while (current.Count > 0)
            {
                List<(int x, int y, Tile tile, Direction dir)> next = [];

                //Console.Clear();
                //Print();

                //int minimumNeighborDistance = current.SelectMany(c => GetNeighbors(c.x, c.y)).Select(x => x.tile.Distance).Min();

                foreach ((int x, int y, Tile tile, Direction dir) in current)
                {
                    if (tile.PartOfPath)
                    {
                        continue;
                    }

                    tile.PartOfPath = true;

                    foreach ((int nx, int ny, Tile neighborTile, Direction _) in GetNeighbors(x, y))
                    {
                        if (neighborTile.Distance == minimumNeighborDistance)
                        {
                            next.Add((nx, ny, neighborTile));
                        }
                    }
                }

                current = next;
            }

            int partOfPathCount = Iter().Count(x => x.tile.PartOfPath);
            return partOfPathCount;
        }

        private static IEnumerable<(Tile tile, Direction dir)> GetDiagonalBackwardsTiles(Direction direction)
        {

        }

        private static Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new Exception("Unknown direction."),
            };
        }

        private void PopulateDistances()
        {
            Tile start = Tiles[Start.x, Start.y];
            start.Distance = 0;
            List<(int x, int y, Tile tile, Direction lastMoved, int givenCost)> queue = [(Start.x, Start.y, start, Direction.Right, 0)];

            while (queue.Count > 0)
            {
                (int x, int y, Tile tile, Direction lastMoved, int givenCost) = queue[0];
                queue.RemoveAt(0);

                if (tile.Distance < givenCost)
                {
                    continue;
                }

                foreach ((int nx, int ny, Tile neighborTile, Direction direction) in GetNeighbors(x, y))
                {
                    if (neighborTile.Type == TileType.Wall)
                    {
                        continue;
                    }

                    int cost = tile.Distance + 1 + GetTurningCost(lastMoved, direction);

                    if (cost == neighborTile.Distance)
                    {
                        neighborTile.GoodDirections.Add(direction);
                    }

                    if (cost < neighborTile.Distance)
                    {
                        neighborTile.Distance = cost;
                        neighborTile.GoodDirections = [direction];

                        if (nx == End.x && ny == End.y)
                        {
                            EndDirection = direction;
                        }

                        queue.Add((nx, ny, neighborTile, direction, cost));
                    }
                }
            }

            Tile end = Tiles[End.x, End.y];
            if (end.Distance == int.MaxValue)
            {
                throw new Exception("No path found.");
            }
        }

        private IEnumerable<(int x, int y, Tile tile, Direction lastMoved)> GetNeighbors(int x, int y)
        {
            if (x > 0)
            {
                yield return (x - 1, y, Tiles[x - 1, y], Direction.Left);
            }

            if (x < Width - 1)
            {
                yield return (x + 1, y, Tiles[x + 1, y], Direction.Right);
            }

            if (y > 0)
            {
                yield return (x, y - 1, Tiles[x, y - 1], Direction.Up);
            }

            if (y < Height - 1)
            {
                yield return (x, y + 1, Tiles[x, y + 1], Direction.Down);
            }
        }

        private static int GetTurningCost(Direction current, Direction towards)
        {
            int count = 0;
            while (current != towards)
            {
                current = TurnCounterClockwise(current);
                count++;
            }

            if (count > 3)
            {
                throw new Exception("Can turn a maximum of 3 times.");
            }

            if (count == 3)
            {
                count = 1;
            }

            return count * 1000;
        }

        private static Direction TurnCounterClockwise(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Down;
                default:
                    throw new Exception("Unknown direction.");
            }
        }

        public static Board ParseInput(string input)
        {
            string[] lines = input.Split("\n");
            int width = lines[0].Length;
            int height = lines.Length;

            Tile[,] tiles = new Tile[width, height];

            (int, int)? start = null;
            (int, int)? end = null;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = lines[y][x];

                    TileType type = c switch
                    {
                        '#' => TileType.Wall,
                        _ => TileType.Empty,
                    };

                    tiles[x, y] = new Tile(type);

                    if (c == 'S')
                    {
                        start = (x, y);
                    }

                    if (c == 'E')
                    {
                        end = (x, y);
                    }
                }
            }

            if (!start.HasValue || !end.HasValue)
            {
                throw new Exception("Start and end positions must be defined.");
            }

            if (start.Value == end.Value)
            {
                throw new Exception("Start and end positions must be different.");
            }

            return new Board(width, height, tiles, start.Value, end.Value);
        }

        private IEnumerable<(int x, int y, Tile tile)> Iter()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return (x, y, Tiles[x, y]);
                }
            }
        }

        public void Print()
        {
            foreach (var (x, y, tile) in Iter())
            {
                char c = tile.Type switch
                {
                    TileType.Empty => '.',
                    TileType.Wall => '#',
                    _ => throw new Exception("Unknown tile type"),
                };

                if (tile.PartOfPath)
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                }

                if ((x, y) == Start)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                }

                if ((x, y) == End)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }

                Console.Write(c);
                Console.BackgroundColor = ConsoleColor.Black;

                if (x == Width - 1)
                {
                    Console.WriteLine();
                }
            }
        }
    }

    class Tile(TileType type)
    {
        public TileType Type = type;
        public int Distance = int.MaxValue;
        public bool PartOfPath = false;
        public List<Direction> GoodDirections = [];
    }

    enum TileType
    {
        Empty,
        Wall,
    }

    enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }
}
