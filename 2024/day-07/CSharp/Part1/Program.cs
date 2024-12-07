namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Equation> equations = ParseInput(input);

            long result = equations.Where(x => x.Verify()).Select(x => x.TestValue).Sum();
            Console.WriteLine($"Result: {result}");
        }

        static List<Equation> ParseInput(string input)
        {
            List<Equation> result = new List<Equation>();

            foreach (string line in input.Split("\n"))
            {
                string[] split = line.Split(": ");
                long testValue = long.Parse(split[0]);

                string[] valuesSplit = split[1].Split(" ");

                List<long> values = new List<long>();
                foreach(string value in valuesSplit)
                {
                    values.Add(long.Parse(value));
                }

                result.Add(new Equation(testValue, values));
            }

            return result;
        }
    }

    class Equation(long testValue, List<long> values)
    {
        public long TestValue = testValue;
        public List<long> Values = values;

        public bool Verify()
        {
            foreach (IEnumerable<int> configuration in OperatorIter(Values.Count))
            {
                long result = Values.First();
                foreach ((int idx, int operatorNum) in configuration.Enumerate())
                {
                    if (operatorNum == 0)
                    {
                        result += Values[idx + 1];
                    }
                    else if (operatorNum == 1)
                    {
                        result *= Values[idx + 1];
                    }
                    else
                    {
                        throw new Exception($"Invalid operatorNum, expected 0 or 1 but found '{operatorNum}'");
                    }
                }

                if (result == TestValue)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<IEnumerable<int>> OperatorIter(int length)
        {
            int[] operators = new int[length - 1];
            for (int i = 0; i < operators.Length; i++)
            {
                operators[i] = 0;
            }

            yield return operators;

            int idx = 0;
            while (operators.Any(x => x != 1))
            {
                if (operators[idx] == 1)
                {
                    operators[idx] = 0;
                    idx++;
                    continue;
                }
                else
                {
                    operators[idx] = 1;
                    idx = 0;
                }

                yield return operators;
            }
        }
    }

    static class LinqExtensions
    {
        public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> src)
        {
            int idx = 0;
            foreach (T item in src)
            {
                yield return (idx, item);
                idx++;
            }
        }
    }
}
