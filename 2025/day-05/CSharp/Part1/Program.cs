namespace Part1;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        (IEnumerable<Range> ranges, IEnumerable<long> ids) = ParseInput(input);
        long res = Solve(ranges, ids);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(IEnumerable<Range> ranges, IEnumerable<long> ids)
    {
        long sum = 0;

        foreach (long id in ids)
        {
            if (IsInAnyRange(ranges, id))
            {
                sum++;
            }
        }
        
        return sum;
    }

    private static bool IsInAnyRange(IEnumerable<Range> ranges, long id)
    {
        return ranges.Any(x => x.IsInRange(id));
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static (IEnumerable<Range>, IEnumerable<long>) ParseInput(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] parts = input.Split("\n\n");
        if (parts.Length != 2)
        {
            throw new Exception("Expected 2 newlines splitting the input sections.");
        }
        
        string rangesInput = parts[0];
        string idsInput = parts[1];
        
        List<Range> ranges = [];
        foreach (string rangeLine in rangesInput.Split("\n"))
        {
            string[] rangeParts = rangeLine.Split("-");
            
            if (rangeParts.Length != 2)
            {
                throw new Exception($"Could not parse '{rangeLine}' expected 2 integers separated by a '-'.");
            }
            
            long start = long.Parse(rangeParts[0]);
            long end = long.Parse(rangeParts[1]);
            
            Range range = new Range(start, end);
            ranges.Add(range);
        }

        List<long> ids = [];
        foreach (string idLine in idsInput.Split("\n"))
        {
            if (!long.TryParse(idLine, out long id))
            {
                throw new Exception($"Could not parse ID '{idLine}'");
            }
            
            ids.Add(id);
        }
        
        return (ranges, ids);
    }
}

internal readonly struct Range(long start, long end)
{
    public readonly long Start = start;
    public readonly long End = end;

    public bool IsInRange(long value)
    {
        return value >= this.Start && value <= this.End;
    }
}
