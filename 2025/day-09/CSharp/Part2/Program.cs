using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        List<Coordinates> coordinates = ParseInput(input);
        Stopwatch sw = Stopwatch.StartNew();
        long res = Solve(coordinates);
        sw.Stop();
        Console.WriteLine($"Result {res} in {sw.ElapsedMilliseconds}ms");
    }

    private static long Solve(List<Coordinates> coordinates)
    {
        long largestArea = long.MinValue;

        Span<Coordinates> span = CollectionsMarshal.AsSpan(coordinates);

        foreach (var pair in span.GetAllCombinations())
        {
            
        }

        foreach (var (first, second) in coordinates.GetAllCombinations())
        {
            long area = (Math.Abs(first.X - second.X) + 1) * (Math.Abs(first.Y - second.Y) + 1);

            if (area <= largestArea)
            {
                continue;
            }
                
            if (!IsValid(first, second, coordinates))
            {
                continue;
            }

            if (area > largestArea)
            {
                largestArea = area;
            }
        }
        
        return largestArea;
    }

    private static bool IsValid(Coordinates first, Coordinates second, List<Coordinates> coordinates)
    {
        int minY = Math.Min(first.Y, second.Y);
        int maxY = Math.Max(first.Y, second.Y);
        int minX = Math.Min(first.X, second.X);
        int maxX = Math.Max(first.X, second.X);

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                Coordinates coord = new Coordinates(j, i);

                var pairs = coordinates.SkipLast(1).Zip(coordinates.Skip(1));
                bool found = false;
                foreach (var pair in pairs)
                {
                    bool insideX = pair.First.X <= coord.X && coord.X <= pair.Second.X;
                    bool insideY = pair.First.Y <= coord.Y && coord.Y <= pair.Second.Y;
                    
                    if (insideX && insideY)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }
        }

        return true;
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
            ReadOnlySpan<char> view = line.AsSpan();
            int comma = view.IndexOf(',');
            int x = int.Parse(view[..comma]);
            int y = int.Parse(view[(comma + 1)..]);
            
            // string[] parts = line.Split(',');
            // int x = int.Parse(parts[0]);
            // int y = int.Parse(parts[1]);
            
            Coordinates coordinate = new Coordinates(x, y);
            coordinates.Add(coordinate);
        }
        
        return coordinates;
    }
}

internal readonly struct Coordinates(int x, int y) : IEquatable<Coordinates>
{
    public readonly int X = x;
    public readonly int Y = y;

    public bool Equals(Coordinates other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Coordinates other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}

internal static class Extensions
{
    public static IEnumerable<(T, T)> GetAllCombinations<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        IList<T> list = source as IList<T> ?? source.ToList();
            
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                yield return (list[i], list[j]);
            }
        }
    }
}

internal static class SpanExtensions
{
    public static SpanCombinationEnumerable<T> GetAllCombinations<T>(this Span<T> source) =>
        new SpanCombinationEnumerable<T>(source);
    
    public static SpanCombinationEnumerable<T> GetAllCombinations<T>(this ReadOnlySpan<T> source) =>
        new SpanCombinationEnumerable<T>(source);
}

internal readonly ref struct SpanCombinationEnumerable<T>
{
    private readonly ReadOnlySpan<T> _span;

    public SpanCombinationEnumerable(ReadOnlySpan<T> span)
    {
        _span = span;
    }

    public SpanCombinationEnumerator<T> GetEnumerator() => new SpanCombinationEnumerator<T>(_span);
}

internal ref struct SpanCombinationEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _i;
    private int _j;
    public (T, T) Current { get; private set; }

    public SpanCombinationEnumerator(ReadOnlySpan<T> span)
    {
        _span = span;
        _i = 0;
        _j = 1;
        Current = default;
    }

    public bool MoveNext()
    {
        if (_i >= _span.Length - 1)
        {
            return false;
        }
        
        Current = (_span[_i], _span[_j]);

        _j++;
        if (_j >= _span.Length)
        {
            _i++;
            _j = _i + 1;
        }
        
        return true;
    }
}
