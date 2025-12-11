using Sprache;

namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Row> rows = ParseInput(input);
        long res = Solve(rows);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(IEnumerable<Row> rows)
    {
        return CountPathsBetweenNodes(rows, "svr", "fft")
               * CountPathsBetweenNodes(rows, "fft", "dac")
               * CountPathsBetweenNodes(rows, "dac", "out");
    }

    private static long CountPathsBetweenNodes(IEnumerable<Row> rows, string start, string end)
    {
        List<string> keys = new List<string>();
        foreach (Row row in rows)
        {
            keys.Add(row.Name);
            keys.AddRange(row.Connections);
        }
        
        Dictionary<string, Node> nodes = keys.Distinct().ToDictionary(x => x, x => new Node(x));
        
        foreach (Row row in rows)
        {
            Node node = nodes[row.Name];

            foreach (string nodeKey in row.Connections)
            {
                Node transitionsTo = nodes[nodeKey];
                transitionsTo.ConnectionsFrom.Add(node);
            }
        }
        
        Dictionary<Node, int> nodeDepth = nodes.Values.ToDictionary(n => n, _ => int.MinValue);
        
        Node endNode = nodes[end];
        nodeDepth[endNode] = 0;
        
        Queue<Node> depthQueue = new Queue<Node>();
        depthQueue.Enqueue(endNode);

        while (depthQueue.Count > 0)
        {
            Node node = depthQueue.Dequeue();
            int depth = nodeDepth[node];
            
            foreach (Node connection in node.ConnectionsFrom)
            {
                int connectionDepth = nodeDepth[connection];
                if (connectionDepth < depth + 1)
                {
                    nodeDepth[connection] = depth + 1;
                    depthQueue.Enqueue(connection);
                }
            }
        }
        
        Dictionary<Node, long> routesToEnd = nodes.Values.ToDictionary(n => n, _ => 0L);
        routesToEnd[endNode] = 1;
        
        Dictionary<Node, bool> nodeVisited = nodes.Values.ToDictionary(n => n, _ => false);
        
        
        PriorityQueue<Node, int> priorityQueue = new PriorityQueue<Node, int>();
        priorityQueue.Enqueue(endNode, 0);

        while (priorityQueue.Count > 0)
        {
            Node node = priorityQueue.Dequeue();

            foreach (Node connection in node.ConnectionsFrom)
            {
                routesToEnd[connection] += routesToEnd[node];

                if (!nodeVisited[connection])
                {
                    nodeVisited[connection] = true;
                    priorityQueue.Enqueue(connection, nodeDepth[connection]);
                }
            }
        }
        
        Node startNode = nodes[start];
        long res = routesToEnd[startNode];
        
        return res;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Row> ParseInput(string input)
    {
        var nameParser = Parse.LetterOrDigit.Repeat(3, 3).Select(string.Concat);
        
        var rowParser =
            from name in nameParser
            from _ in Parse.String(": ")
            from connections in nameParser.DelimitedBy(Parse.Char(' '))
            select new Row(name, connections);

        var res = rowParser
            .DelimitedBy(Parse.LineEnd)
            .Token()
            .End()
            .Parse(input) ?? throw new Exception("Could not parse input.");
        
        return res;
    }
}

internal class Row(string name, IEnumerable<string> connections)
{
    public readonly string Name = name;
    public readonly List<string> Connections = connections as List<string> ?? connections.ToList();
}

internal class Node(string name)
{
    public readonly string Name = name;
    public readonly List<Node> ConnectionsFrom = new();
}
