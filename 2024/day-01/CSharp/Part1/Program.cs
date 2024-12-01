namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);

            (List<int> left, List<int> right) = GetLeftAndRightLists(input);

            int result = left.OrderBy(x => x).Zip(right.OrderBy(x => x), (x, y) => Math.Abs(x - y)).Sum();
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
