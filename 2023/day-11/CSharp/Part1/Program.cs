using System.Text;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            Cell[,] universe = InputParser.ParseInput(input);
            Cell[] galaxyIterator = GalaxyIterator(universe).ToArray();

            int totalDistance = 0;
            for (int i = 0; i < galaxyIterator.Length - 1; i++)
            {
                for (int j = i + 1; j < galaxyIterator.Length; j++)
                {
                    Cell cell = galaxyIterator[i];
                    Cell otherCell = galaxyIterator[j];

                    int distance = Math.Abs(cell.X - otherCell.X) + Math.Abs(cell.Y - otherCell.Y);
                    totalDistance += distance;
                }
            }


        }

        private static IEnumerable<Cell> GalaxyIterator(Cell[,] universe)
        {
            int width = universe.GetLength(0);
            int height = universe.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Cell cell = universe[i, j];
                    if (cell.Type == Type.Galaxy)
                    {
                        yield return cell;
                    }
                }
            }

            yield break;
        }
    }

    public enum Type
    {
        Empty,
        Galaxy
    }

    public class Cell
    {
        public int X;
        public int Y;
        public Type Type;

        public Cell(int x, int y, char symbol)
        {
            X = x;
            Y = y;
            Type = SymbolToType(symbol);
        }

        private static Type SymbolToType(char symbol)
        {
            switch (symbol)
            {
                case '.':
                    return Type.Empty;
                case '#':
                    return Type.Galaxy;
                default:
                    throw new Exception($"Cannot convert '{symbol}'");
            }
        }
    }

    public static class InputParser
    {
        public static Cell[,] ParseInput(string input)
        {
            List<string> lines = input.Split("\n").Where(x => x != "").ToList();
            List<string> expandedLines = ExpandUniverse(lines);

            int width = expandedLines[0].Length;
            int height = expandedLines.Count;

            Cell[,] cells = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j] = new Cell(i, j, expandedLines[j][i]);
                }
            }

            return cells;
        }

        private static List<string> ExpandUniverse(IEnumerable<string> lines)
        {
            List<int> expandColumnIndexes = Enumerable.Range(0, lines.First().Count()).Where(index => lines.All(line => line[index] == '.')).ToList();
            List<int> expandRowIndexes = lines.Select((val, index) => (val, index)).Where(x => x.val.All(c => c == '.')).Select(x => x.index).ToList();

            List<string> expandedLines = lines.Select(line =>
            {
                StringBuilder builder = new StringBuilder(line);

                foreach (int index in expandColumnIndexes.Reverse<int>())
                {
                    builder.Insert(index, ".");
                }

                return builder.ToString();
            }).ToList();

            int width = expandedLines.First().Length;

            foreach (int index in expandRowIndexes.Reverse<int>())
            {
                expandedLines.Insert(index, new string('.', width));
            }

            return expandedLines;
        }
    }
}
