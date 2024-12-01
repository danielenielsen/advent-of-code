using System.Linq;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);

            (List<int> left, List<int> right) = GetLeftAndRightLists(input);

            Dictionary<int, int> rightSimilarity = right.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            int result = left.Select(x => rightSimilarity.TryGetValue(x, out int similarity) ? x * similarity : 0).Sum();
            Console.WriteLine(result);
        }

        private static (List<int>, List<int>) GetLeftAndRightLists(string input)
        {
            List<string> lines = input.Split("\n").Where(x => x != "").ToList();

            List<int> left = new List<int>();
            List<int> right = new List<int>();

            foreach (string line in lines)
            {
                List<string> split = line.Split("   ").ToList();

                if (split.Count != 2)
                {
                    throw new Exception("The split had a length of " + split.Count);
                }

                int leftNumber = int.Parse(split[0]);
                int rightNumber = int.Parse(split[1]);

                left.Add(leftNumber);
                right.Add(rightNumber);
            }

            return (left, right);
        }
    }
}
