namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);

            int result = board.FindAndCountAntiNodes(print: true);
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

        private void MarkAntiNodes(bool print)
        {
            if (print)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Print();
            }

            foreach ((int x, int y, Tile tile) in Iter())
            {
                if (!tile.Frequency.HasValue)
                {
                    continue;
                }

                char frequency = tile.Frequency.Value;

                foreach ((int otherX, int otherY, Tile otherTile) in Iter())
                {
                    if (x == otherX && y == otherY)
                    {
                        continue;
                    }

                    if (!otherTile.Frequency.HasValue)
                    {
                        continue;
                    }

                    char otherFrequency = otherTile.Frequency.Value;

                    if (frequency != otherFrequency)
                    {
                        continue;
                    }

                    int xDiff = otherX - x;
                    int yDiff = otherY - y;

                    int antiNodeX = otherX + xDiff;
                    int antiNodeY = otherY + yDiff;

                    if (antiNodeX < 0 || antiNodeX >= Width || antiNodeY < 0 || antiNodeY >= Height)
                    {
                        continue;
                    }

                    Tiles[antiNodeX, antiNodeY].IsAntiNode = true;

                    if (print)
                    {
                        PrintUpdatePoint(antiNodeX, antiNodeY);
                    }
                }
            }

            if (print)
            {
                Console.SetCursorPosition(Width, Height);
                Console.WriteLine();
                Console.CursorVisible = true;
            }
        }

        public int FindAndCountAntiNodes(bool print)
        {
            MarkAntiNodes(print);

            int count = 0;
            foreach ((int _, int _, Tile tile) in Iter())
            {
                if (tile.IsAntiNode)
                {
                    count++;
                }
            }

            return count;
        }

        private IEnumerable<(int x, int y, Tile tile)> Iter()
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

            Tile[,] tiles = new Tile[width, height];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    char c = lines[i][j];
                    char? frequency = c switch
                    {
                        '.' => null,
                        _ => c,
                    };

                    Tile tile = new Tile(frequency);
                    tiles[j, i] = tile;
                }
            }

            return new Board(width, height, tiles);
        }

        public void Print()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Tile tile = Tiles[j, i];
                    char charToWrite = tile.Frequency switch
                    {
                        null => '.',
                        _ => (char)tile.Frequency,
                    };

                    if (tile.IsAntiNode)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }

                    Console.Write(charToWrite);
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine();
            }
        }

        private void PrintUpdatePoint(int x, int y)
        {
            Tile tile = Tiles[x, y];
            char charToWrite = tile.Frequency switch
            {
                null => '.',
                _ => (char)tile.Frequency,
            };

            if (tile.IsAntiNode)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }

            Console.SetCursorPosition(x, y);

            Console.Write(charToWrite);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    class Tile(char? frequency)
    {
        public char? Frequency = frequency;
        public bool IsAntiNode = false;
    }
}
