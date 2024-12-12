namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);

            int result = board.CalculateTotalCost();
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

        public int CalculateTotalCost()
        {
            int totalCost = 0;

            foreach ((int x, int y, Tile tile) in Iter())
            {
                if (tile.Visited)
                {
                    continue;
                }

                totalCost += CalculateRegionCost(x, y);
            }

            return totalCost;
        }

        private int CalculateRegionCost(int x, int y)
        {
            char label = Tiles[x, y].Label;

            int area = 0;
            int perimeter = 0;

            List<(int x, int y)> queue = [(x, y)];

            while (queue.Count > 0)
            {
                (int currentX, int currentY) = queue[0];
                queue.RemoveAt(0);

                Tile current = Tiles[currentX, currentY];

                if (current.Visited)
                {
                    continue;
                }

                current.Visited = true;
                area++;

                if (currentX == 0)
                {
                    perimeter++;
                }

                if (currentX == Width - 1)
                {
                    perimeter++;
                }

                if (currentY == 0)
                {
                    perimeter++;
                }

                if (currentY == Height - 1)
                {
                    perimeter++;
                }

                foreach ((int nx, int ny) in GetNeighbors(currentX, currentY))
                {
                    Tile neighbor = Tiles[nx, ny];

                    if (neighbor.Label != label)
                    {
                        perimeter++;
                        continue;
                    }

                    queue.Add((nx, ny));
                }
            }

            return area * perimeter;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            if (x > 0)
            {
                yield return (x - 1, y);
            }
            if (x < Width - 1)
            {
                yield return (x + 1, y);
            }
            if (y > 0)
            {
                yield return (x, y - 1);
            }
            if (y < Height - 1)
            {
                yield return (x, y + 1);
            }
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

        public static Board ParseInput(string input)
        {
            string[] lines = input.Split("\n");
            int width = lines[0].Length;
            int height = lines.Length;

            Tile[,] tiles = new Tile[width, height];
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile(line[x]);
                }
            }

            return new Board(width, height, tiles);
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Tiles[x, y].Label);
                }
                Console.WriteLine();
            }
        }
    }

    class Tile(char label)
    {
        public char Label = label;
        public bool Visited = false;
    }
}
