using Sprache;

namespace Part1
{
    internal class Program
    {
        public static List<int> digits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static List<char> digitChars = digits.ConvertAll(x => x.ToString()[0]);
        public static List<char> noMeaningChars = new List<char> { '.', '\n' };

        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);

            List<char> otherChars = input.ToCharArray().Distinct().Except(digitChars).Except(noMeaningChars).ToList();

            Matrix matrix = GetCharMatrix(input);

            CheckMatrixCells(matrix, otherChars);
            List<int> numbers = parseNumbers.Parse(matrix.OutputCombinedString());
            int result = numbers.Sum();
        }

        private static readonly Parser<List<int>> parseNumbers =
            from numbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            select numbers.Select(x => int.Parse(x)).ToList();

        public static Matrix GetCharMatrix(string str)
        {
            List<string> rows = str.Split("\n").Where(x => x != "").ToList();

            int width = rows.First().Length;
            int height = rows.Count;

            MatrixCell[,] cells = new MatrixCell[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j] = new MatrixCell(i, j, rows[j][i]);
                }
            }

            return new Matrix(width, height, cells);
        }

        public static List<MatrixCell> GetNeighborNumberCellsHelper(Matrix matrix, MatrixCell cell)
        {
            List<MatrixCell> result = new List<MatrixCell> { cell };
            cell.Visited = true;

            if (cell.X > 0)
            {
                MatrixCell otherCell = matrix.Cells[cell.X - 1, cell.Y];
                if (!otherCell.Visited && digitChars.Contains(otherCell.Value))
                {
                    result.AddRange(GetNeighborNumberCells(matrix, otherCell));
                }
            }

            if (cell.X < matrix.Width - 1)
            {
                MatrixCell otherCell = matrix.Cells[cell.X + 1, cell.Y];
                if (!otherCell.Visited && digitChars.Contains(otherCell.Value))
                {
                    result.AddRange(GetNeighborNumberCells(matrix, otherCell));
                }
            }

            return result;
        }

        public static List<MatrixCell> GetNeighborNumberCells(Matrix matrix, MatrixCell cell)
        {
            List<MatrixCell> result = GetNeighborNumberCellsHelper(matrix, cell);
            result.ForEach(x => x.Visited = false);
            return result;
        }

        public static bool CellHasCharSurroundingIt(Matrix matrix, MatrixCell cell, List<char> chars)
        {
            List<int> numbers = new List<int> { -1, 0, 1 };

            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = 0; j < numbers.Count; j++)
                {
                    if (numbers[i] == 0 && numbers[j] == 0)
                    {
                        continue;
                    }

                    int newX = cell.X + numbers[i];
                    int newY = cell.Y + numbers[j];

                    if (newX < 0 || newX >= matrix.Width || newY < 0 || newY >= matrix.Height)
                    {
                        continue;
                    }

                    MatrixCell otherCell = matrix.Cells[newX, newY];
                    if (chars.Contains(otherCell.Value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void CheckMatrixCells(Matrix matrix, List<char> symbols)
        {
            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    MatrixCell cell = matrix.Cells[i, j];

                    if (cell.Valid)
                    {
                        continue;
                    }

                    if (!digitChars.Contains(cell.Value))
                    {
                        continue;
                    }

                    List<MatrixCell> neighboringCells = GetNeighborNumberCells(matrix, cell);
                    bool hasNeighboringSymbol = neighboringCells.Any(x => CellHasCharSurroundingIt(matrix, x, symbols));

                    if (hasNeighboringSymbol)
                    {
                        neighboringCells.ForEach(x => x.Valid = true);
                    }
                }
            }

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    MatrixCell cell = matrix.Cells[i, j];
                    if (!cell.Valid)
                    {
                        cell.Value = ' ';
                    }
                }
            }
        }
    }

    public class MatrixCell
    {
        public int X;
        public int Y;
        public char Value;
        public bool Valid = false;
        public bool Visited = false;

        public MatrixCell(int x, int y, char value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }

    public class Matrix
    {
        public int Width;
        public int Height;
        public MatrixCell[,] Cells;

        public Matrix(int width, int height, MatrixCell[,] cells)
        {
            Width = width;
            Height = height;
            Cells = cells;
        }

        public void Print(int row = -1)
        {
            if (row >= 0)
            {
                for (int i = 0; i < Width; i++)
                {
                    Console.Write(Cells[i, row].Value);
                }
                Console.WriteLine();
                return;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    MatrixCell cell = Cells[j, i];
                    Console.Write(cell.Value);
                }
                Console.WriteLine();
            }
        }

        public string OutputCombinedString()
        {
            string result = "";

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    MatrixCell cell = Cells[j, i];
                    result += cell.Value;
                }
                result += " ";
            }

            return result.Trim();
        }
    }
}
