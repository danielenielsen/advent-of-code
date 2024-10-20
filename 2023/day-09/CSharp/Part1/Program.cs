using Sprache;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            List<OasisHistory> histories = InputParser.ParseInput(input);

            long result = histories.Select(x => Extrapolate(x.History)).Sum();
        }

        private static long Extrapolate(IEnumerable<long> list)
        {
            List<long> lastItems = new List<long>();

            IEnumerable<long> differences = list;
            while (true)
            {
                lastItems.Add(differences.Last());
                differences = differences.GetPairs().Select(x => x.Item2 - x.Item1);

                if (differences.AllAreEqual(0))
                {
                    break;
                }
            }

            long currentNum = 0;
            foreach (long item in lastItems)
            {
                currentNum += item;
            }

            return currentNum;
        }
    }

    public class OasisHistory
    {
        public IEnumerable<long> History;

        public OasisHistory(IEnumerable<long> history)
        {
            History = history;
        }
    }

    public static class InputParser
    {
        public static List<OasisHistory> ParseInput(string input)
        {
            return historyListParser.Parse(input);
        }

        private static readonly Parser<OasisHistory> historyParser =
            from numbers in (
                    from minus in Parse.Char('-').Optional()
                    from number in Parse.Digit.AtLeastOnce().Text()
                    select minus.IsDefined ? long.Parse("-" + number) : long.Parse(number)
                ).DelimitedBy(Parse.Char(' '))
            select new OasisHistory(numbers);

        private static readonly Parser<List<OasisHistory>> historyListParser =
            from histories in historyParser.DelimitedBy(Parse.Char('\n'))
            select histories.ToList();
    }

    public static class LinqExtension
    {
        public static IEnumerable<(T, T)> GetPairs<T>(this IEnumerable<T> source)
        {
            return source.SkipLast(1).Zip(source.Skip(1), (a, b) => (a, b));
        }

        public static bool AllAreEqual<T>(this IEnumerable<T> source, T item) where T : IComparable<T>
        {
            return source.All(element => element.Equals(item));
        }
    }
}
