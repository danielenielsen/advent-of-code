using Sprache;

namespace Part2
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
            int result = GetResult(matrix);
        }

        private static readonly Parser<List<int>> parseNumbers =
            from numbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            select numbers.Select(int.Parse).ToList();

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

        public static List<MatrixCell> GetSurroundingNumberCells(Matrix matrix, MatrixCell cell)
        {
            List<MatrixCell> results = new List<MatrixCell>();
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
                    if (digitChars.Contains(otherCell.Value))
                    {
                        results.Add(otherCell);
                    }
                }
            }

            return results;
        }

        public static int GetNumberFromMatrixCell(Matrix matrix, MatrixCell cell)
        {
            List<MatrixCell> numberCells = GetNeighborNumberCells(matrix, cell);

            List<MatrixCell> sortedCells = numberCells.Select(x => (matrix.Width * x.Y + x.X, x)).OrderBy(x => x.Item1).Select(x => x.x).ToList();
            return int.Parse(new string(sortedCells.Select(x => x.Value).ToArray()));
        }

        public static int GetResult(Matrix matrix)
        {
            List<MatrixCell> gearCells = new List<MatrixCell>();
            for (int i = 0; i < matrix.Width; i++)
            {
                for(int j = 0;j < matrix.Height; j++)
                {
                    MatrixCell cell = matrix.Cells[i,j];
                    if (cell.Value == '*')
                    {
                        gearCells.Add(cell);
                    }
                }
            }

            int result = 0;
            foreach (MatrixCell cell in gearCells)
            {
                List<MatrixCell> numberCells = GetSurroundingNumberCells(matrix, cell);

                List<MatrixCell> verifiedNumberCells = new List<MatrixCell>();
                foreach (MatrixCell numberCell in numberCells)
                {
                    List<MatrixCell> neighboringNumberCells = GetNeighborNumberCells(matrix, numberCell);
                    if (verifiedNumberCells.Except(neighboringNumberCells).Count() == verifiedNumberCells.Count)
                    {
                        verifiedNumberCells.Add(numberCell);
                    }
                }

                if (verifiedNumberCells.Count == 2)
                {
                    int num1 = GetNumberFromMatrixCell(matrix, verifiedNumberCells[0]);
                    int num2 = GetNumberFromMatrixCell(matrix, verifiedNumberCells[1]);

                    result += num1 * num2;
                }
            }

            return result;
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
    }
}
