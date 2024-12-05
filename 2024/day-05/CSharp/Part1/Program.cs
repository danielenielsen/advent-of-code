namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "");
            (List<Rule> rules, List<Update> updates) = ParseInput(input);

            int result = updates.Where(x => x.IsValid(rules)).Select(x => x.GetMiddleNumber()).Sum();
            Console.WriteLine($"Result: {result}");
        }

        static (List<Rule>, List<Update>) ParseInput(string input)
        {
            string[] split = input.Split("\n\n");

            string rules_text = split[0];
            string updates_text = split[1];

            List<Rule> rules = new List<Rule>();
            foreach (string line in rules_text.Split("\n"))
            {
                string[] parts = line.Split("|");
                int before = int.Parse(parts[0]);
                int after = int.Parse(parts[1]);
                rules.Add(new Rule(before, after));
            }

            List<Update> updates = new List<Update>();
            foreach (string line in updates_text.Split("\n"))
            {
                List<int> numbers = new List<int>();
                foreach (string number in line.Split(","))
                {
                    numbers.Add(int.Parse(number));
                }
                updates.Add(new Update(numbers));
            }

            return (rules, updates);
        }
    }

    class Rule(int before, int after)
    {
        public int Before = before;
        public int After = after;
    }

    class Update(List<int> numbers)
    {
        public List<int> Numbers = numbers;

        public int GetMiddleNumber()
        {
            if (Numbers.Count % 2 == 0)
            {
                throw new Exception("Numbers count is even");
            }

            int idx = Numbers.Count / 2;
            return Numbers[idx];
        }

        public bool IsValid(List<Rule> rules)
        {
            for (int i = 0; i < Numbers.Count; i++)
            {
                foreach (Rule rule in rules)
                {
                    if (rule.Before != Numbers[i])
                    {
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        if (Numbers[j] == rule.After)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
