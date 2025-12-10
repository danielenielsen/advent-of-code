namespace Part1;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        List<Coordinates> coordinates = ParseInput(input);
        long res = Solve(coordinates);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(List<Coordinates> coordinates)
    {
        long largestArea = long.MinValue;

        for (int i = 0; i < coordinates.Count - 1; i++)
        {
            for (int j = i + 1; j < coordinates.Count; j++)
            {
                Coordinates first = coordinates[i];
                Coordinates second = coordinates[j];
                long area = (Math.Abs(first.X - second.X) + 1) * (Math.Abs(first.Y - second.Y) + 1);

                if (area > largestArea)
                {
                    largestArea = area;
                }
            }
        }
        
        return largestArea;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static List<Coordinates> ParseInput(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split('\n');
        List<Coordinates> coordinates = new List<Coordinates>(lines.Length);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            long x = long.Parse(parts[0]);
            long y = long.Parse(parts[1]);
            Coordinates coordinate = new Coordinates(x, y);
            coordinates.Add(coordinate);
        }
        
        return coordinates;
    }
}

internal readonly struct Coordinates(long x, long y)
{
    public readonly long X = x;
    public readonly long Y = y;
}
