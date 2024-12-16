namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);
            board.Print();
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;

        public (int x, int y) Start;
        public (int x, int y) End;

        private Board(int width, int height, Tile[,] tiles, (int, int) start, (int, int) end)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
            Start = start;
            End = end;
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
