namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Stone> stones = ParseInput(input);

            Dictionary<(long stoneNum, int blink), long> stoneBlinkLookup = new Dictionary<(long stoneNum, int blink), long>();

            long result = stones.Select(x => Blink(stoneBlinkLookup, x.Number, 75)).Sum();
            Console.WriteLine($"Result: {result}");
        }

        static long Blink(Dictionary<(long stoneNum, int blink), long> stoneBlinkLookup, long stoneNum, int blink)
        {
            if (blink == 0)
            {
                return 1;
            }

            if (stoneBlinkLookup.TryGetValue((stoneNum, blink), out long res))
            {
                return res;
            }

            if (stoneNum == 0)
            {
                res = Blink(stoneBlinkLookup, 1, blink - 1);
            }
            else if (stoneNum.ToString().Length % 2 == 0)
            {
                string num = stoneNum.ToString();
                int half = num.Length / 2;
                string first = num[..half];
                string second = num[half..];

                if (first.Contains("-") || second.Contains("-"))
                {
                    throw new Exception("Not supposed to be below 0");
                }

                long firstNumber = long.Parse(first);
                long secondNumber = long.Parse(second);

                res = Blink(stoneBlinkLookup, firstNumber, blink - 1) + Blink(stoneBlinkLookup, secondNumber, blink - 1);
            }
            else
            {
                res = Blink(stoneBlinkLookup, stoneNum * 2024, blink - 1);
            }

            stoneBlinkLookup[(stoneNum, blink)] = res;
            return res;
        }

        static List<Stone> ParseInput(string input)
        {
            List<Stone> stones = new List<Stone>();

            string[] split = input.Split(" ");
            foreach (string s in split)
            {
                long num = long.Parse(s);
                stones.Add(new Stone(num));
            }

            return stones;
        }
    }

    class Stone(long number)
    {
        public long Number = number;
    }
}
