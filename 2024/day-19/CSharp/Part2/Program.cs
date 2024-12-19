namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            (List<string> availableTowels, List<string> patternsToMatch) = ParseInput(input);

            Dictionary<string, long> patternLookup = new Dictionary<string, long>();
            long matches = patternsToMatch.Sum(x => MatchCount(patternLookup, availableTowels, x));
            Console.WriteLine($"Match count: {matches}");
        }

        static (List<string> availableTowels, List<string> patternsToMatch) ParseInput(string input)
        {
            string[] parts = input.Split("\n\n");

            List<string> availableTowels = parts[0].Split(", ").OrderByDescending(x => x.Length).ToList();
            List<string> patternsToMatch = parts[1].Split("\n").ToList();

            return (availableTowels, patternsToMatch);
        }

        static long MatchCount(Dictionary<string, long> patternLookup, List<string> availableTowels, string pattern)
        {
            if (patternLookup.TryGetValue(pattern, out long res))
            {
                return res;
            }

            List<string> candidates = availableTowels.Where(pattern.StartsWith).ToList();
            long count = 0;

            foreach (string towel in candidates)
            {
                if (towel == pattern)
                {
                    count += 1;
                }

                string remainingPattern = pattern[towel.Length..];
                count += MatchCount(patternLookup, availableTowels, remainingPattern);
            }

            patternLookup.Add(pattern, count);
            return count;
        }
    }
}
