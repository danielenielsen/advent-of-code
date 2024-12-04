namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "");
            Board board = ParseInput(input);

            int result = board.CountXmas();
            Console.WriteLine($"Result: {result}");
        }

        static Board ParseInput(string input)
        {
            string[] lines = input.Split('\n');

            int width = lines[0].Length;
            int height = lines.Length;

            Board board = new Board(width, height);

            for (int y = 0; y < height; y++)
            {
                string line = lines[y];

                for (int x = 0; x < width; x++)
                {
                    board.Chars[x, y] = line[x];
                }
            }

            return board;
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public char[,] Chars;

        public Board(int width, int height)
        {
            Width = width;
            Height = height;

            Chars = new char[width, height];
        }

        public int CountXmas()
        {
            int count = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x + 2 >= Width || y + 2 >= Height)
                    {
                        continue;
                    }

                    bool firstDiagonal = Chars[x, y] == 'M' && Chars[x + 1, y + 1] == 'A' && Chars[x + 2, y + 2] == 'S' || Chars[x, y] == 'S' && Chars[x + 1, y + 1] == 'A' && Chars[x + 2, y + 2] == 'M';
                    bool secondDiagonal = Chars[x + 2, y] == 'M' && Chars[x + 1, y + 1] == 'A' && Chars[x, y + 2] == 'S' || Chars[x + 2, y] == 'S' && Chars[x + 1, y + 1] == 'A' && Chars[x, y + 2] == 'M';

                    if (firstDiagonal && secondDiagonal)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Chars[x, y]);
                }

                Console.WriteLine();
            }
        }
    }
}
