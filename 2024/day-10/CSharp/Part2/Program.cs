namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);

            int result = board.CalculateScore();
            Console.WriteLine($"Result: {result}");
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;

        private Board(int width, int height, Tile[,] tiles)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        public static Board ParseInput(string input)
        {
            string[] split = input.Split("\n");
            int width = split[0].Length;
            int height = split.Length;

            Tile[,] tiles = new Tile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int num = int.Parse(split[y][x].ToString());
                    tiles[x, y] = new Tile(num);
                }
            }

            return new Board(width, height, tiles);
        }

        public int CalculateScore()
        {
            int score = 0;
            foreach ((int x, int y, Tile tile) in Iter())
            {
                if (tile.Height == 0)
                {
                    score += CalculateTrailheadScore(x, y);
                }
            }
            return score;
        }

        private int CalculateTrailheadScore(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new Exception($"Invalid coordinates, got ({x},{y}).");
            }

            Tile start = Tiles[x, y];
            if (start.Height != 0)
            {
                throw new Exception($"Can only start on heights of 0, got {start.Height}.");
            }

            List<(int x, int y)> queue = new List<(int x, int y)>
            {
                (x, y)
            };

            int count = 0;

            while (queue.Count > 0)
            {
                (int currentX, int currentY) = queue[0];
                queue.RemoveAt(0);

                Tile current = Tiles[currentX, currentY];
                if (current.Height == 9)
                {
                    count++;
                    continue;
                }

                foreach ((int neighborX, int neighborY) in GetNeighbors(currentX, currentY))
                {
                    Tile neighbor = Tiles[neighborX, neighborY];
                    if (neighbor.Height == current.Height + 1)
                    {
                        queue.Add((neighborX, neighborY));
                    }
                }
            }

            return count;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            if (x > 0)
            {
                yield return (x - 1, y);
            }

            if (y > 0)
            {
                yield return (x, y - 1);
            }

            if (x < Width - 1)
            {
                yield return (x + 1, y);
            }

            if (y < Height - 1)
            {
                yield return (x, y + 1);
            }
        }

        public IEnumerable<(int x, int y, Tile tile)> Iter()
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
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Tiles[x, y].Height);
                }
                Console.WriteLine();
            }
        }
    }

    class Tile(int height)
    {
        public int Height = height;
    }
}
