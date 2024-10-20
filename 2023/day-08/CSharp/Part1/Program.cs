using Sprache;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            DirectionInformation directionInformation = InputParser.ParseInput(input);
            Dictionary<(string, Direction), string> locationDict = GetLocationDictionary(directionInformation.Nodes);
            string start = "AAA";
            string end = "ZZZ";

            string current = start;
            long steps = 0;
            foreach (Direction direction in GetInfiniteDirectionIterator(directionInformation.Directions))
            {
                if (current == end)
                {
                    break;
                }

                steps++;

                current = locationDict[(current, direction)];
            }

            Console.WriteLine(steps);
        }

        public static IEnumerable<Direction> GetInfiniteDirectionIterator(List<Direction> directions)
        {
            while (true)
            {
                foreach (Direction direction in directions)
                {
                    yield return direction;
                }
            }
        }

        public static Dictionary<(string, Direction), string> GetLocationDictionary(List<(string, string, string)> nodes)
        {
            Dictionary<(string, Direction), string> result = new Dictionary<(string, Direction), string>();
            foreach (var node in nodes)
            {
                result[(node.Item1, Direction.Left)] = node.Item2;
                result[(node.Item1, Direction.Right)] = node.Item3;
            }

            return result;
        }
    }

    public enum Direction
    {
        Left,
        Right
    }

    public class DirectionInformation
    {
        public List<Direction> Directions;
        public List<(string, string, string)> Nodes;

        public DirectionInformation(List<Direction> directions, List<(string, string, string)> nodes)
        {
            Directions = directions;
            Nodes = nodes;
        }
    }

    public static class InputParser
    {
        public static DirectionInformation ParseInput(string input)
        {
            return parseDirectionInformation.Parse(input);
        }

        private static Direction CharToDirection(char c)
        {
            switch (c)
            {
                case 'L':
                    return Direction.Left;
                case 'R':
                    return Direction.Right;
                default:
                    throw new Exception($"The char '{c}' does not match a direction");
            }
        }

        private static readonly Parser<(string, string, string)> parseNode =
            from nodeName in Parse.Letter.AtLeastOnce().Text()
            from str in Parse.String(" = (")
            from left in Parse.Letter.AtLeastOnce().Text()
            from str2 in Parse.String(", ")
            from right in Parse.Letter.AtLeastOnce().Text()
            from str3 in Parse.String(")")
            select (nodeName, left, right);

        private static readonly Parser<DirectionInformation> parseDirectionInformation =
            from directions in Parse.Char('L').Or(Parse.Char('R')).AtLeastOnce()
            from newLines in Parse.String("\n\n")
            from nodes in parseNode.DelimitedBy(Parse.Char('\n'))
            select new DirectionInformation(directions.Select(CharToDirection).ToList(), nodes.ToList());
    }
}
