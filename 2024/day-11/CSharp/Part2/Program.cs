namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Stone> stones = ParseInput(input);

            for (int i = 0; i < 75; i++)
            {
                stones = Blink(stones);
            }

            int result = stones.Count;
            Console.WriteLine($"Result: {result}");
        }

        static List<Stone> Blink(List<Stone> currentStones)
        {
            List<Stone> stones = currentStones.ToList();

            for (int i = stones.Count - 1; i >= 0; i--)
            {
                if (stones[i].Number == 0)
                {
                    stones[i].Number += 1;
                    continue;
                }

                if (stones[i].Number.ToString().Count() % 2 == 0)
                {
                    string num = stones[i].Number.ToString();
                    int half = num.Length / 2;
                    string first = num[..half];
                    string second = num[half..];

                    if (first.Contains("-") || second.Contains("-"))
                    {
                        throw new Exception("Not supposed to be below 0");
                    }

                    long firstNumber = long.Parse(first);
                    long secondNumber = long.Parse(second);

                    stones[i].Number = firstNumber;
                    Stone newStone = new Stone(secondNumber);
                    stones.Insert(i + 1, newStone);

                    continue;
                }

                stones[i].Number *= 2024;
            }

            return stones;
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
