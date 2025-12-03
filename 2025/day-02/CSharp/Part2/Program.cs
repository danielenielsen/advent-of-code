namespace Part2;

internal class Program
{
    internal static void Main(string[] args)
    {
        string input = "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124"; //GetInput();
        IEnumerable<Range> ranges = ParseInput(input);
        long result = Solve(ranges);
        Console.WriteLine($"Result: {result}");
    }

    private static long Solve(IEnumerable<Range> ranges)
    {
        long sum = 0;

        foreach (Range range in ranges)
        {
            for (long i = range.Start; i <= range.End; i++)
            {
                if (NumberIsRepeated(i))
                {
                    sum += i;
                }
            }
        }

        return sum;
    }

    private static bool NumberIsRepeated(long number)
    {
        string numberString = number.ToString();

        for (int i = 1; i < numberString.Length / 2; i++)
        {
            if (HasRepeatingPattern(numberString, i))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasRepeatingPattern(string number, int patternLength)
    {
        if (number.Length % patternLength != 0)
        {
            return false;
        }

        string pattern = number[..patternLength];

        int i = patternLength;
        while (i < number.Length)
        {
            if (number[i..(i + patternLength)] != pattern)
            {
                return false;
            }

            i += patternLength;
        }

        return true;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Range> ParseInput(string input)
    {
        string[] lines = input.Trim().Split(",");
        List<Range> res = new List<Range>(lines.Length);

        foreach (string line in lines)
        {
            string[] parts = line.Split("-");
            long start = long.Parse(parts[0]);
            long end = long.Parse(parts[1]);
            Range range = new Range(start, end);
            res.Add(range);
        }

        return res;
    }
}

internal readonly struct Range(long start, long end)
{
    public long Start { get; } = start;
    public long End { get; } = end;
}