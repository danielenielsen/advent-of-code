namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Range> ranges = ParseInput(input);
        long res = Solve(ranges);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(IEnumerable<Range> ranges)
    {
        IEnumerable<Range> mergedRanges = MergeAllOverlappingRanges(ranges);
        return mergedRanges.Select(x => x.End - x.Start + 1).Sum();
    }

    private static IEnumerable<Range> MergeAllOverlappingRanges(IEnumerable<Range> ranges)
    {
        List<Range> mergedRanges = ranges.ToList();

        while (true)
        {
            bool anyMerges = false;

            for (int i = 0; i < mergedRanges.Count - 1; i++)
            {
                if (anyMerges)
                {
                    break;
                }
                
                for (int j = i + 1; j < mergedRanges.Count; j++)
                {
                    Range range1 = mergedRanges[i];
                    Range range2 = mergedRanges[j];
                    Range? newRange = CombineRanges(range1, range2);

                    if (newRange != null)
                    {
                        anyMerges = true;
                        
                        mergedRanges.RemoveAt(j);
                        mergedRanges.RemoveAt(i);
                        mergedRanges.Add(newRange.Value);
                        
                        break;
                    }
                }
            }

            if (!anyMerges)
            {
                break;
            }
        }

        return mergedRanges;
    }

    private static Range? CombineRanges(Range range1, Range range2)
    {
        // They are identical
        if (range1.Start == range2.Start && range1.End == range2.End)
        {
            return range1;
        }
        
        // range1 contains range2
        if (range1.Start < range2.Start && range1.End > range2.End)
        {
            return range1;
        }
        
        // range2 contains range1
        if (range2.Start < range1.Start && range2.End > range1.End)
        {
            return range2;
        }
        
        // No overlap
        if (range1.End < range2.Start || range2.End < range1.Start)
        {
            return null;
        }
        
        // Overlap
        long start = Math.Min(range1.Start, range2.Start);
        long end = Math.Max(range1.End, range2.End);
        return new Range(start, end);
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Range> ParseInput(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] parts = input.Split("\n\n");
        if (parts.Length != 2)
        {
            throw new Exception("Expected 2 newlines splitting the input sections.");
        }
        
        string rangesInput = parts[0];
        
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

        return ranges;
    }
}

internal readonly struct Range(long start, long end)
{
    public readonly long Start = start;
    public readonly long End = end;
}
