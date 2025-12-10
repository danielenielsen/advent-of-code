using System.Text;
using Sprache;

namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Row> rows = ParseInput(input);
        int res = Solve(rows);
        Console.WriteLine($"Result: {res}");
    }

    private static int Solve(IEnumerable<Row> rows)
    {
        int sum = 0;

        foreach (Row row in rows)
        {
            sum += SolveRow(row);
        }
        
        return sum;
    }

    private static int SolveRow(Row row)
    {
        int lightCount = row.Lights.Count;

        Dictionary<string, Node> nodes = new();
        bool[] lightCombinations = new bool[lightCount];

        nodes[lightCombinations.AsString()] = new Node(lightCombinations.ToArray());
        while (true)
        {
            int i = 0;
            while (i < lightCount && lightCombinations[i])
            {
                lightCombinations[i] = false;
                i++;
            }

            if (i >= lightCount)
            {
                break;
            }
            
            lightCombinations[i] = true;

            string key = lightCombinations.AsString();
            nodes[key] = new Node(lightCombinations.ToArray());
        }

        foreach (Node node in nodes.Values)
        {
            foreach (Button button in row.Buttons)
            {
                bool[] lights = node.Lights.ToArray();
                
                foreach (int number in button.Numbers)
                {
                    lights[number] = !lights[number];
                }

                string transitionsToKey = lights.AsString();
                node.Transitions.Add(nodes[transitionsToKey]);
            }
        }
        
        Dictionary<Node, bool> nodesVisited = nodes.Values.ToDictionary(x => x, _ => false);
        Dictionary<Node, int> nodeDistance = nodes.Values.ToDictionary(x => x, _ => int.MaxValue);

        string rootKey = new bool[lightCount].AsString();
        Node rootNode = nodes[rootKey];
        
        nodeDistance[rootNode] = 0;
        Queue<Node> queue = new();
        queue.Enqueue(rootNode);

        while (queue.Count > 0)
        {
            Node node = queue.Dequeue();
            int currentDistance = nodeDistance[node];

            foreach (Node transitionNode in node.Transitions)
            {
                if (nodesVisited[transitionNode])
                {
                    continue;
                }
                
                nodesVisited[transitionNode] = true;
                nodeDistance[transitionNode] = currentDistance + 1;
                queue.Enqueue(transitionNode);
            }
        }

        Node resNode = nodes[row.Lights.AsString()];
        int res = nodeDistance[resNode];
        return res;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Row> ParseInput(string input)
    {
        Func<char, Light> lightMap = c => c switch
        {
            '.' => Light.Off,
            '#' => Light.On,
            _ => throw new ArgumentOutOfRangeException(),
        };
        
        var lightsParser =
            from _1 in Parse.Char('[')
            from lights in Parse.Chars('.', '#').Select(lightMap).AtLeastOnce()
            from _2 in Parse.Char(']')
            select lights;
        
        var buttonParser =
            from _1 in Parse.Char('(')
            from numbers in Parse.Number.Select(int.Parse).DelimitedBy(Parse.Char(','))
            from _2 in Parse.Char(')')
            select new Button(numbers);
        
        var joltParser =
            from _1 in Parse.Char('{')
            from numbers in Parse.Number.Select(int.Parse).DelimitedBy(Parse.Char(','))
            from _2 in Parse.Char('}')
            select new Jolts(numbers);

        var rowParser =
            from lightsCount in lightsParser
            from _1 in Parse.WhiteSpace.Many()
            from buttons in buttonParser.DelimitedBy(Parse.WhiteSpace.Many())
            from _2 in Parse.WhiteSpace.Many()
            from jolts in joltParser
            select new Row(lightsCount, buttons, jolts);

        var res = rowParser
            .DelimitedBy(Parse.LineEnd)
            .TrailingWhitespaces()
            .End()
            .Parse(input);

        return res;
    }
}

internal static class Extensions
{
    public static Parser<T> TrailingWhitespaces<T>(this Parser<T> parser)
    {
        return
            from result in parser
            from _ in Parse.WhiteSpace.Many()
            select result;
    }

    public static string AsString(this IEnumerable<Light> lights)
    {
        string result = "";

        foreach (Light light in lights)
        {
            char c = light switch
            {
                Light.Off => '.',
                Light.On => '#',
                _ => throw new ArgumentOutOfRangeException(),
            };
            
            result += c;
        }
        
        return result;
    }

    public static string AsString(this IEnumerable<bool> lights)
    {
        bool[] lightArray = lights as bool[] ?? lights.ToArray();
        StringBuilder sb = new StringBuilder(lightArray.Length);

        foreach (bool light in lightArray)
        {
            char c = light switch
            {
                false => '.',
                true => '#',
            };
            
            sb.Append(c);
        }
        
        return sb.ToString();
    }
}

internal enum Light
{
    Off,
    On,
}

internal class Row(IEnumerable<Light> lights, IEnumerable<Button> buttons, Jolts jolts)
{
    public readonly List<Light> Lights = lights as List<Light> ?? lights.ToList();
    public readonly List<Button> Buttons = buttons as List<Button> ?? buttons.ToList();
    public readonly Jolts Jolts = jolts;
}

internal readonly struct Button(IEnumerable<int> numbers)
{
    public readonly List<int> Numbers = numbers as List<int> ?? numbers.ToList();
}

internal readonly struct Jolts(IEnumerable<int> numbers)
{
    public readonly List<int> Numbers = numbers as List<int> ?? numbers.ToList();
}

internal class Graph
{
    
}

internal class Node(bool[] lights)
{
    public readonly bool[] Lights = lights;
    public readonly List<Node> Transitions = new();
}
