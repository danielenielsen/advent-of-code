namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);

            List<string> lines = input.Split("\n").Where(x => x != "").ToList();

            int count = 0;
            foreach (string line in lines)
            {
                List<int> numbers = line.Split(" ").Select(int.Parse).ToList();

                if (ReportIsSafe(numbers))
                {
                    count++;
                    continue;
                }

                for (int i = 0; i < numbers.Count; i++)
                {
                    List<int> copy = numbers.ToList();
                    copy.RemoveAt(i);

                    if (ReportIsSafe(copy))
                    {
                        count++;
                        break;
                    }
                }
            }

            Console.WriteLine($"Result: {count}");
        }

        private static bool ReportIsSafe(List<int> numbers)
        {
            bool numbersAscending = numbers.SequenceEqual(numbers.OrderBy(x => x));
            bool numbersDescending = numbers.SequenceEqual(numbers.OrderByDescending(x => x));

            if (!numbersAscending && !numbersDescending)
            {
                return false;
            }

            bool goodDiffs = numbers.SkipLast(1).Zip(numbers.Skip(1), (x, y) => Math.Abs(x - y)).All(x => x > 0 && x <= 3);

            if (!goodDiffs)
            {
                return false;
            }

            return true;
        }
    }
}