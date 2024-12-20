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
        private int _width;
        private int _height;
        private Tile[,] _tiles;

        private (int x, int y) _startPos;
        private (int x, int y) _endPos;

        private Board(int width, int height, Tile[,] tiles, (int x, int y) startPos, (int x, int y) endPos)
        {
            _width = width;
            _height = height;
            _tiles = tiles;
            _startPos = startPos;
            _endPos = endPos;
        }

        private void ValidatePathfinding()
        {
            foreach (Tile tile in Iter())
            {
                if (tile.Type == TileType.Empty && tile.Distance == int.MaxValue)
                {
                    throw new Exception("Not all empty tiles have been visited.");
                }
            }
        }

        private IEnumerable<Tile> Iter()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    yield return _tiles[x, y];
                }
            }
        }

        public static Board ParseInput(string input)
        {
            string[] lines = input.Split("\n");

            int width = lines[0].Length;
            int height = lines.Length;

            Tile[,] tiles = new Tile[width, height];
            (int x, int y)? startPos = null;
            (int x, int y)? endPos = null;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = lines[y][x];

                    TileType type = c switch
                    {
                        '.' or 'S' or 'E' => TileType.Empty,
                        '#' => TileType.Wall,
                        _ => throw new Exception($"Invalid tile type '{c}'."),
                    };

                    if (c == 'S')
                    {
                        startPos = (x, y);
                    }
                    
                    if (c == 'E')
                    {
                        endPos = (x, y);
                    }

                    tiles[x, y] = new Tile(x, y, type);
                }
            }

            if (!startPos.HasValue)
            {
                throw new Exception("Start position not found.");
            }

            if (!endPos.HasValue)
            {
                throw new Exception("End position not found.");
            }

            if (startPos.Value == endPos.Value)
            {
                throw new Exception("Start and end positions are the same.");
            }

            return new Board(width, height, tiles, startPos.Value, endPos.Value);
        }

        public void Print()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Tile tile = _tiles[x, y];
                    char c = tile.Type switch {
                        TileType.Empty => '.',
                        TileType.Wall => '#',
                        _ => throw new Exception($"Invalid tile type '{tile.Type}'."),
                    };

                    if ((x, y) == _startPos)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }

                    if ((x, y) == _endPos)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }

                    Console.Write(c);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
        }
    }

    class Tile(int x, int y, TileType type)
    {
        public int X = x;
        public int Y = y;

        public TileType Type = type;
        public int Distance = int.MaxValue;
    }

    enum TileType
    {
        Empty,
        Wall,
    }
}
