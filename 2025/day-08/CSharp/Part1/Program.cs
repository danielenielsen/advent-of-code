using System.Diagnostics;

namespace Part1;

internal static class Program
{
    private static void Main()
    {
        RunTests();
        string input = GetInput();
        List<Coordinates> coords = ParseInput(input);
        long res = Solve(coords, 1000);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(List<Coordinates> coordinates, int combineAmount)
    {
        return Time("Solve body", () =>
        {
            List<(Coordinates first, Coordinates second, long distance)> combinations = [];
            for (int i = 0; i < coordinates.Count; i++)
            {
                for (int j = i + 1; j < coordinates.Count; j++)
                {
                    Coordinates first = coordinates[i];
                    Coordinates second = coordinates[j];
                    long distance = first.GetDistance(second);
                    combinations.Add((first, second, distance));
                }
            }

            var closestPairs = combinations
                .OrderBy(x => x.Item3)
                .Take(combineAmount);
            
            List<List<Coordinates>> circuits = [];

            foreach ((Coordinates first, Coordinates second, _) in closestPairs)
            {
                int? res1 = null;
                int? res2 = null;

                for (int i = 0; i < circuits.Count; i++)
                {
                    if (circuits[i].Contains(first))
                    {
                        res1 = i;
                    }
                    
                    if (circuits[i].Contains(second))
                    {
                        res2 = i;
                    }
                }

                if (res1 == null && res2 == null)
                {
                    circuits.Add([first, second]);
                    continue;
                }

                if (res1 != null && res2 != null)
                {
                    if (res1.Value == res2.Value)
                    {
                        continue;
                    }
                    
                    circuits[res1.Value].AddRange(circuits[res2.Value]);
                    circuits.RemoveAt(res2.Value);
                    continue;
                }
                
                if (res1 != null)
                {
                    circuits[res1.Value].Add(second);
                    continue;
                }

                if (res2 != null)
                {
                    circuits[res2.Value].Add(first);
                    continue;
                }
            }
        
            return circuits.Select(x => x.Count).OrderByDescending(x => x).Take(3).Aggregate(1, (x, y) => x * y);
        });
    }

    private static T Time<T>(string name, Func<T> action)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        T res = action();
        stopwatch.Stop();
        
        Console.WriteLine($"{name}: Took {stopwatch.ElapsedMilliseconds}ms");
        return res;
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
            long z = long.Parse(parts[2]);
            
            Coordinates coord = new Coordinates(x, y, z);
            coordinates.Add(coord);
        }
        
        return coordinates;
    }

    private static void RunTests()
    {
        RunTest("Test 1", "162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689", 10, 40);
    }

    private static void RunTest(string testName, string input, int combineAmount, long expected)
    {
        List<Coordinates> coordinates = ParseInput(input);
        long actual = Solve(coordinates, combineAmount);
        if (actual != expected)
        {
            throw new Exception($"{testName}: Expected {expected}, got {actual}.");
        }
    }
}

internal static class CoordinateExtensions
{
    public static long GetDistance(this Coordinates coords, Coordinates other)
    {
        long firstPart = Math.Abs(coords.X - other.X);
        long secondPart = Math.Abs(coords.Y - other.Y);
        long thirdPart = Math.Abs(coords.Z - other.Z);
        return firstPart * firstPart + secondPart * secondPart + thirdPart * thirdPart;
    }
}

internal readonly struct Coordinates(long x, long y, long z) : IEquatable<Coordinates>
{
    public readonly long X = x;
    public readonly long Y = y;
    public readonly long Z = z;

    public bool Equals(Coordinates other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is Coordinates other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}
