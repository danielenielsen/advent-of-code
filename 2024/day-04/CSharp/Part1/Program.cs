namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "");
            Board board = ParseInput(input);

            int horizontalCount = board.CountNormalXmas();

            board.FlipHorizontally();
            int horizontalReverseCount = board.CountNormalXmas();
            board.FlipHorizontally();

            board.Transpose();
            int verticalCount = board.CountNormalXmas();

            board.FlipHorizontally();
            int verticalReverseCount = board.CountNormalXmas();
            board.FlipHorizontally();
            board.Transpose();

            int normalDiagonals = board.CountDiagonals();

            board.FlipHorizontally();
            int reverseDiagonals = board.CountDiagonals();
            board.FlipHorizontally();

            board.FlipVertically();

            int verticalDiagonals = board.CountDiagonals();

            board.FlipHorizontally();
            int verticalReverseDiagonals = board.CountDiagonals();
            board.FlipHorizontally();

            int result = horizontalCount + horizontalReverseCount + verticalCount + verticalReverseCount + normalDiagonals + reverseDiagonals + verticalDiagonals + verticalReverseDiagonals;
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

        public int CountNormalXmas()
        {
            int count = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x + 3 < Width && Chars[x, y] == 'X' && Chars[x + 1, y] == 'M' && Chars[x + 2, y] == 'A' &&
                        Chars[x + 3, y] == 'S')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public int CountDiagonals()
        {
            int count = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x + 3 < Width && y + 3 < Height && Chars[x, y] == 'X' && Chars[x + 1, y + 1] == 'M' &&
                        Chars[x + 2, y + 2] == 'A' && Chars[x + 3, y + 3] == 'S')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void FlipHorizontally()
        {
            for (int x = 0; x < Width / 2; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    (Chars[x, y], Chars[Width - x - 1, y]) = (Chars[Width - x - 1, y], Chars[x, y]);
                }
            }
        }

        public void FlipVertically()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height / 2; y++)
                {
                    (Chars[x, y], Chars[x, Height - y - 1]) = (Chars[x, Height - y - 1], Chars[x, y]);
                }
            }
        }

        public void Transpose()
        {
            char[,] newChars = new char[Height, Width];


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    newChars[y, x] = Chars[x, y];
                }
            }

            Chars = newChars;
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
