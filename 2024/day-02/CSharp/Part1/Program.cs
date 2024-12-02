namespace Part1
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
                bool numbersAscending = numbers.SequenceEqual(numbers.OrderBy(x => x));
                bool numbersDescending = numbers.SequenceEqual(numbers.OrderByDescending(x => x));

                if (!numbersAscending && !numbersDescending)
                {
                    continue;
                }

                bool goodDiffs = numbers.SkipLast(1).Zip(numbers.Skip(1), (x, y) => Math.Abs(x - y)).All(x => x > 0 && x <= 3);

                if (!goodDiffs)
                {
                    continue;
                }

                count++;
            }

            Console.WriteLine($"Result: {count}");
        }
    }
}
