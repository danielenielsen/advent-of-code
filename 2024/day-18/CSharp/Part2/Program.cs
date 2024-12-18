namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<(int x, int y)> byteCoordinates = ParseInput(input);
            Board board = new Board(71, 71);

            foreach ((int x, int y) coords in byteCoordinates)
            {
                board.SetByte(coords.x, coords.y);

                if (!board.CanReachEnd())
                {
                    Console.WriteLine($"Result: {coords.x},{coords.y}");
                    break;
                }
            }
        }

        static List<(int x, int y)> ParseInput(string input)
        {
            List<(int, int)> result = [];

            string[] lines = input.Split("\n");

            foreach (string line in lines)
            {
                string[] parts = line.Split(",");
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                result.Add((x, y));
            }

            return result;
        }
    }

    class Board
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Tile[,] _tiles;

        public Board(int width, int height)
        {
            _width = width;
            _height = height;
            _tiles = new Tile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _tiles[x, y] = new Tile(x, y);
                }
            }
        }

        public bool CanReachEnd()
        {
            Tile start = _tiles[0, 0];
            start.Distance = 0;
            List<Tile> queue = [start];

            while (queue.Count > 0)
            {
                Tile currentTile = queue[0];
                queue.RemoveAt(0);

                foreach (Tile neighborTile in GetNeighbors(currentTile))
                {
                    if (neighborTile.HasByte)
                    {
                        continue;
                    }

                    if (neighborTile.Distance > currentTile.Distance + 1)
                    {
                        neighborTile.Distance = currentTile.Distance + 1;
                        queue.Add(neighborTile);
                    }
                }
            }

            bool res = _tiles[_width - 1, _height - 1].Distance != int.MaxValue;

            foreach (Tile tile in Iterator())
            {
                tile.Distance = int.MaxValue;
            }

            return res;
        }

        private IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            if (tile.X > 0)
            {
                yield return _tiles[tile.X - 1, tile.Y];
            }

            if (tile.X < _width - 1)
            {
                yield return _tiles[tile.X + 1, tile.Y];
            }

            if (tile.Y > 0)
            {
                yield return _tiles[tile.X, tile.Y - 1];
            }

            if (tile.Y < _height - 1)
            {
                yield return _tiles[tile.X, tile.Y + 1];
            }
        }

        public void SetByte(int x, int y)
        {
            _tiles[x, y].HasByte = true;
        }

        public void SetByte(IEnumerable<(int x, int y)> coordinates)
        {
            foreach ((int x, int y) coords in coordinates)
            {
                SetByte(coords.x, coords.y);
            }
        }

        private IEnumerable<Tile> Iterator()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    yield return _tiles[x, y];
                }
            }
        }

        public void Print()
        {
            foreach (Tile tile in Iterator())
            {
                char c = tile.HasByte ? '#' : '.';
                Console.Write(c);

                if (tile.X == _width - 1)
                {
                    Console.WriteLine();
                }
            }
        }
    }

    class Tile(int x, int y)
    {
        public int X = x;
        public int Y = y;
        public bool HasByte = false;
        public int Distance = int.MaxValue;
    }
}
