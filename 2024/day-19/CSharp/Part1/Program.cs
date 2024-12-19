namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            (List<string> availableTowels, List<string> patternsToMatch) = ParseInput(input);

            Dictionary<string, bool> patternLookup = new Dictionary<string, bool>();
            int matches = patternsToMatch.Count(x => IsMatch(patternLookup, availableTowels, x));
            Console.WriteLine($"Matches: {matches}");
        }

        static (List<string> availableTowels, List<string> patternsToMatch) ParseInput(string input)
        {
            string[] parts = input.Split("\n\n");

            List<string> availableTowels = parts[0].Split(", ").OrderByDescending(x=> x.Length).ToList();
            List<string> patternsToMatch = parts[1].Split("\n").ToList();

            return (availableTowels, patternsToMatch);
        }

        static bool IsMatch(Dictionary<string, bool> patternLookup, List<string> availableTowels, string pattern)
        {
            if (pattern.Length == 0)
            {
                return true;
            }

            if (patternLookup.TryGetValue(pattern, out bool res))
            {
                return res;
            }

            bool isMatch = false;
            List<string> candidates = availableTowels.Where(pattern.StartsWith).ToList();
            foreach (string towel in candidates)
            {
                string remainingPattern = pattern[towel.Length..];

                if (!IsMatch(patternLookup, availableTowels, remainingPattern))
                {
                    continue;
                }

                isMatch = true;
                break;
            }

            patternLookup.Add(pattern, isMatch);
            return isMatch;
        }
    }
}
