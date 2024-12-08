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

            //Console.WriteLine(input);
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

        private List<(int x, int y)> GetPath()
        {
            List<(int x, int y)> result = new List<(int x, int y)>();



            return result;
        }

        public IEnumerable<(int x, int y, Tile tile)> Iter()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    yield return (j, i, Tiles[j, i]);
                }
            }
        }

        public static Board ParseInput(string input)
        {
            string[] lines = input.Split("\n");
            int width = lines[0].Length;
            int height = lines.Length;

            if (lines.Any(x => x.Length != width))
            {
                throw new Exception($"Expected all lines to have a width of {width}.");
            }

            Tile[,] tiles = new Tile[width, height];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    char c = lines[i][j];
                    int heatLoss = int.Parse(c.ToString());
                    tiles[j, i] = new Tile(heatLoss);
                }
            }

            Board board = new Board(width, height, tiles);

            if (board.Iter().Any(x => x.tile == null))
            {
                throw new Exception("Expected all tiles to be non-null");
            }

            return board;
        }

        public void Print()
        {
            int prevY = 0;
            foreach ((int x, int y, Tile tile) in Iter())
            {
                if (y != prevY)
                {
                    Console.WriteLine();
                    prevY = y;
                }

                Console.Write(tile.HeatLoss);
            }
        }
    }

    class Tile(int heatLoss)
    {
        public int HeatLoss = heatLoss;
    }
}
