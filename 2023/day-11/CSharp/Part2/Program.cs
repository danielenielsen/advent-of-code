using System.Text;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            Cell[] galaxies = InputParser.ParseInput(input).ToArray();

            long totalDistance = 0;
            for (int i = 0; i < galaxies.Length - 1; i++)
            {
                for (int j = i + 1; j < galaxies.Length; j++)
                {
                    Cell cell = galaxies[i];
                    Cell otherCell = galaxies[j];

                    long distance = Math.Abs(cell.X - otherCell.X) + Math.Abs(cell.Y - otherCell.Y);
                    totalDistance += distance;
                }
            }


        }
    }

    public class Cell
    {
        public int X;
        public int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static class InputParser
    {
        public static List<Cell> ParseInput(string input)
        {
            List<string> lines = input.Split("\n").Where(x => x != "").ToList();
            (List<int> expandColumnIndexes, List<int> expandRowIndexes) = FindExpandUniverseIndexes(lines);

            int width = lines[0].Length;
            int height = lines.Count;

            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (lines[j][i] == '#')
                    {
                        int x = i + expandColumnIndexes.Where(x => x < i).Count() * 999_999;
                        int y = j + expandRowIndexes.Where(x => x < j).Count() * 999_999;

                        cells.Add(new Cell(x, y));
                    }
                }
            }

            return cells;
        }

        private static (List<int>, List<int>) FindExpandUniverseIndexes(IEnumerable<string> lines)
        {
            List<int> expandColumnIndexes = Enumerable.Range(0, lines.First().Count()).Where(index => lines.All(line => line[index] == '.')).ToList();
            List<int> expandRowIndexes = lines.Select((val, index) => (val, index)).Where(x => x.val.All(c => c == '.')).Select(x => x.index).ToList();

            return (expandColumnIndexes, expandRowIndexes);
        }
    }
}
