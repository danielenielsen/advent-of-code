using Sprache;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            DirectionInformation directionInformation = InputParser.ParseInput(input);
            Dictionary<(string, Direction), string> locationDict = GetLocationDictionary(directionInformation.Nodes);
            List<string> start = directionInformation.Nodes.Where(x => x.Item1[2] == 'A').Select(x => x.Item1).ToList();

            List<long> startValueLoopLengths = new List<long>();
            
            foreach (string val in start)
            {
                string current = val;
                long steps = 0;
                foreach (Direction direction in GetInfiniteDirectionIterator(directionInformation.Directions))
                {
                    if (current[2] == 'Z')
                    {
                        startValueLoopLengths.Add(steps);
                        break;
                    }

                    steps++;

                    current = locationDict[(current, direction)];
                }
            }

            long result = FindLCM(startValueLoopLengths);
        }

        static long FindLCM(List<long> numbers)
        {
            long lcm = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                lcm = LCM(lcm, numbers[i]);
            }
            return lcm;
        }

        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long LCM(long a, long b)
        {
            return a / GCD(a, b) * b;
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
