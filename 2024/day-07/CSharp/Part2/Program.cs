namespace Part2
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
                foreach (string value in valuesSplit)
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
            foreach (IEnumerable<int> configuration in OperatorIter(Values.Count - 1))
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
                    else if (operatorNum == 2)
                    {
                        result = long.Parse(result.ToString() + Values[idx + 1].ToString());
                    }
                    else
                    {
                        throw new Exception($"Invalid operatorNum, expected 0, 1 or 2 but found '{operatorNum}'");
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
            int[] operators = new int[length];
            for (int i = 0; i < operators.Length; i++)
            {
                operators[i] = 0;
            }

            yield return operators;

            int combinationCount = 1;
            int idx = 0;
            while (operators.Any(x => x != 2))
            {
                if (operators[idx] == 2)
                {
                    operators[idx] = 0;
                    idx++;
                    continue;
                }
                else if (operators[idx] == 1 || operators[idx] == 0)
                {
                    operators[idx]++;
                    idx = 0;
                }
                else
                {
                    throw new Exception($"Operators should only have a value of 1, 2 or 3, but found '{operators[idx]}'");
                }

                combinationCount++;
                yield return operators;
            }

            int expectedCombinations = (int)Math.Pow(3, length);
            if (combinationCount != expectedCombinations)
            {
                throw new Exception($"Expected {expectedCombinations} combinations, but actually produced {combinationCount}");
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
