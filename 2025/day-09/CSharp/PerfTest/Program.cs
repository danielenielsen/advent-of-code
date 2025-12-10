using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace PerfTest;

internal static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<Benchmarks>();
        return;
        
        string input = GetInput();
        List<(int, int)> coordinates = Time(() => ParseInput1(input));
        List<(int, int)> coordinates2 = Time(() => ParseInput2(input));
        List<(int, int)> coordinates3 = Time(() => ParseInput3(input));
        List<(int, int)> coordinates4 = Time(() => ParseInput4(input));
        
        Console.WriteLine(coordinates.Count);
        Console.WriteLine(coordinates2.Count);
        Console.WriteLine(coordinates3.Count);
        Console.WriteLine(coordinates4.Count);

        bool equal = coordinates.SequenceEqual(coordinates2) && coordinates2.SequenceEqual(coordinates3) && coordinates3.SequenceEqual(coordinates4);
        if (!equal)
        {
            throw new Exception();
        }
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static List<(int, int)> ParseInput1(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split('\n');
        List<(int, int)> coordinates = new List<(int, int)>(lines.Length);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            coordinates.Add((x, y));
        }
        
        return coordinates;
    }
    
    private static List<(int, int)> ParseInput2(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split('\n');
        List<(int, int)> coordinates = new List<(int, int)>(lines.Length);

        foreach (string line in lines)
        {
            ReadOnlySpan<char> view = line.AsSpan();
            int comma = view.IndexOf(',');
            int x = int.Parse(view[..comma]);
            int y = int.Parse(view[(comma + 1)..]);
            coordinates.Add((x, y));
        }
        
        return coordinates;
    }
    
    private static List<(int, int)> ParseInput3(string input)
    {
        ReadOnlySpan<char> inputView = input.AsSpan();

        int newLineCount = 0;
        foreach (char c in inputView)
        {
            if (c == '\n')
            {
                newLineCount++;
            }
        }
        
        List<(int, int)> coordinates = new List<(int, int)>(newLineCount - 1);

        ReadOnlySpan<char> currentView = inputView;
        while (true)
        {
            if (currentView.Length == 0)
            {
                break;
            }
            
            int newLineIndex = currentView.IndexOf('\n');
            
            ReadOnlySpan<char> lineView = currentView[..newLineIndex];
            int commaIndex = lineView.IndexOf(',');
            int x = int.Parse(lineView[..commaIndex]);
            int y = int.Parse(lineView[(commaIndex + 1)..]);
            
            coordinates.Add((x, y));
            currentView = currentView[(newLineIndex + 1)..];
        }
        
        return coordinates;
    }

    private static List<(int, int)> ParseInput4(string input)
    {
        ReadOnlySpan<char> span = input.AsSpan();

        // You can guess an initial capacity if you want,
        // but a default List is usually fine.
        var coordinates = new List<(int x, int y)>();

        int lineStart = 0;
        int commaIndex = -1;

        // Note: i <= span.Length so we can treat "end of string" as "end of line"
        for (int i = 0; i <= span.Length; i++)
        {
            bool endOfLine = i == span.Length || span[i] == '\n';

            if (!endOfLine)
            {
                if (span[i] == ',')
                {
                    commaIndex = i;
                }

                continue;
            }

            // Empty line? (e.g., trailing newline)
            if (i == lineStart)
            {
                lineStart = i + 1;
                commaIndex = -1;
                continue;
            }

            if (commaIndex < 0)
            {
                throw new FormatException("Line does not contain a comma.");
            }

            // x spans from lineStart to commaIndex - 1
            ReadOnlySpan<char> xSpan = span.Slice(lineStart, commaIndex - lineStart);

            // y spans from commaIndex + 1 to i - 1
            ReadOnlySpan<char> ySpan = span.Slice(commaIndex + 1, i - (commaIndex + 1));

            int x = int.Parse(xSpan);
            int y = int.Parse(ySpan);

            coordinates.Add((x, y));

            // Move to the next line
            lineStart = i + 1;
            commaIndex = -1;
        }

        return coordinates;
    }
    
    private static void Time(Action action)
    {
        Stopwatch sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        Console.WriteLine($"{nameof(action)} {sw.ElapsedTicks}");
    }
    
    private static T Time<T>(Func<T> func)
    {
        Stopwatch sw = Stopwatch.StartNew();
        T res = func();
        sw.Stop();
        Console.WriteLine($"{nameof(func)} {sw.ElapsedTicks}");
        return res;
    }
}

[MemoryDiagnoser]
public class Benchmarks
{
    private string input;
    
    [GlobalSetup]
    public void Setup()
    {
        input = File.ReadAllText("../../../../../../../../../input.txt");
    }
    
    [Benchmark]
    public List<(int, int)> ParseInput1()
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split('\n');
        List<(int, int)> coordinates = new List<(int, int)>(lines.Length);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            coordinates.Add((x, y));
        }
        
        return coordinates;
    }
    
    [Benchmark]
    public List<(int, int)> ParseInput2()
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split('\n');
        List<(int, int)> coordinates = new List<(int, int)>(lines.Length);

        foreach (string line in lines)
        {
            ReadOnlySpan<char> view = line.AsSpan();
            int comma = view.IndexOf(',');
            int x = int.Parse(view[..comma]);
            int y = int.Parse(view[(comma + 1)..]);
            coordinates.Add((x, y));
        }
        
        return coordinates;
    }
    
    [Benchmark]
    public List<(int, int)> ParseInput3()
    {
        ReadOnlySpan<char> inputView = input.AsSpan();

        int newLineCount = 0;
        foreach (char c in inputView)
        {
            if (c == '\n')
            {
                newLineCount++;
            }
        }
        
        List<(int, int)> coordinates = new List<(int, int)>(newLineCount);

        ReadOnlySpan<char> currentView = inputView;
        while (true)
        {
            if (currentView.Length == 0)
            {
                break;
            }
            
            int newLineIndex = currentView.IndexOf('\n');
            
            ReadOnlySpan<char> lineView = currentView[..newLineIndex];
            int commaIndex = lineView.IndexOf(',');
            int x = int.Parse(lineView[..commaIndex]);
            int y = int.Parse(lineView[(commaIndex + 1)..]);
            
            coordinates.Add((x, y));
            currentView = currentView[(newLineIndex + 1)..];
        }
        
        return coordinates;
    }

    [Benchmark]
    public List<(int, int)> ParseInput4()
    {
        ReadOnlySpan<char> span = input.AsSpan();

        // You can guess an initial capacity if you want,
        // but a default List is usually fine.
        var coordinates = new List<(int x, int y)>();

        int lineStart = 0;
        int commaIndex = -1;

        // Note: i <= span.Length so we can treat "end of string" as "end of line"
        for (int i = 0; i <= span.Length; i++)
        {
            bool endOfLine = i == span.Length || span[i] == '\n';

            if (!endOfLine)
            {
                if (span[i] == ',')
                {
                    commaIndex = i;
                }

                continue;
            }

            // Empty line? (e.g., trailing newline)
            if (i == lineStart)
            {
                lineStart = i + 1;
                commaIndex = -1;
                continue;
            }

            if (commaIndex < 0)
            {
                throw new FormatException("Line does not contain a comma.");
            }

            // x spans from lineStart to commaIndex - 1
            ReadOnlySpan<char> xSpan = span.Slice(lineStart, commaIndex - lineStart);

            // y spans from commaIndex + 1 to i - 1
            ReadOnlySpan<char> ySpan = span.Slice(commaIndex + 1, i - (commaIndex + 1));

            int x = int.Parse(xSpan);
            int y = int.Parse(ySpan);

            coordinates.Add((x, y));

            // Move to the next line
            lineStart = i + 1;
            commaIndex = -1;
        }

        return coordinates;
    }
}
