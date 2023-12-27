using Sprache;
using System.Diagnostics;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            List<SpringRow> rows = InputParser.ParseInput(input);

            //SpringRow row = InputParser.rowParser.Parse("???????? 1,3");
            //SpringRow row = InputParser.rowParser.Parse("?????????? 1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("...??????????...##??? 1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse(".????? 1,3");
            //SpringRow row = InputParser.rowParser.Parse("???.#????#.? 1,1,1,1");
            //SpringRow row = InputParser.rowParser.Parse("????????????????????? 1,1,2,1,1,2");
            SpringRow row = InputParser.rowParser.Parse("???????????????????????????????? 1,1,2,1,1,2,1,1,2");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int result = CountCombinations(row);
            stopwatch.Stop();
            Console.WriteLine(result);
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000d);

            //int result = rows.Select(CountCombinations).Sum();

            
        }

        private static int CountCombinations(SpringRow row)
        {
            if (!row.Validate())
            {
                return 0;
            }

            int unknownPos = row.Springs.FindIndex(x => x == SpringCondition.Unknown);
            if (unknownPos == -1)
            {
                //Console.WriteLine(string.Join("", row.Springs.Select(x => x == SpringCondition.Damaged ? "#" : ".")));
                return 1;
            }

            SpringRow copy1 = row.Copy();
            SpringRow copy2 = row.Copy();

            copy1.Springs[unknownPos] = SpringCondition.Functional;
            copy2.Springs[unknownPos] = SpringCondition.Damaged;

            return CountCombinations(copy1) + CountCombinations(copy2);
    }
    }

    public enum SpringCondition
    {
        Unknown,
        Damaged,
        Functional
    }

    public class SpringRow
    {
        public List<SpringCondition> Springs;
        public List<int> SpringLogs;
        public int NumberOfDamagedSprings;
        public int NumberOfFunctionalSprings;

        public SpringRow(List<SpringCondition> springs, List<int> springLogs)
        {
            Springs = springs;
            SpringLogs = springLogs;

            int total = springs.Count;
            int amountOfDamaged = springLogs.Sum();

            NumberOfDamagedSprings = amountOfDamaged;
            NumberOfFunctionalSprings = total - amountOfDamaged;
        }

        public SpringRow Copy()
        {
            return new SpringRow(Springs.ToList(), SpringLogs.ToList());
        }

        public bool Validate()
        {
            Dictionary<SpringCondition, int> countDict = Springs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            if (countDict.ContainsKey(SpringCondition.Damaged) && countDict[SpringCondition.Damaged] > NumberOfDamagedSprings)
            {
                return false;
            }

            if (countDict.ContainsKey(SpringCondition.Functional) && countDict[SpringCondition.Functional] > NumberOfFunctionalSprings)
            {
                return false;
            }

            List<SpringCondition> springsUntilUnknown = Springs.TakeWhile(x => x != SpringCondition.Unknown).ToList();
            List<int> damagedGroups = FindDamagedGroups(springsUntilUnknown);
            List<int> logSubset = SpringLogs.Take(damagedGroups.Count).ToList();

            if (damagedGroups.Count == 0)
            {
                return true;
            }

            int lastDamagedGroup = damagedGroups.Last();
            int lastLogSubset = logSubset.Last();

            damagedGroups = damagedGroups.SkipLast(1).ToList();
            logSubset = logSubset.SkipLast(1).ToList();

            if (!damagedGroups.SequenceEqual(logSubset))
            {
                return false;
            }

            if (lastLogSubset < lastDamagedGroup)
            {
                return false;
            }

            return true;
        }

        private List<int> FindDamagedGroups(List<SpringCondition> springs)
        {
            if (springs.Any(x => x == SpringCondition.Unknown))
            {
                throw new Exception("Must not contain unknown springs");
            }

            List<int> groups = new List<int>();
            while (true)
            {
                int amountOfNotDamagedInFront = springs.TakeWhile(x => x != SpringCondition.Damaged).Count();
                springs = springs.Skip(amountOfNotDamagedInFront).ToList();

                int amountOfDamagedInFront = springs.TakeWhile(x => x == SpringCondition.Damaged).Count();

                if (amountOfDamagedInFront == 0)
                {
                    break;
                }

                groups.Add(amountOfDamagedInFront);
                springs = springs.Skip(amountOfDamagedInFront).ToList();
            }

            return groups;
        }
    }

    public static class InputParser
    {
        public static List<SpringRow> ParseInput(string input)
        {
            return allRowsParser.Parse(input);
        }

        private static SpringCondition CharToSpring(char c)
        {
            switch (c)
            {
                case '?':
                    return SpringCondition.Unknown;
                case '#':
                    return SpringCondition.Damaged;
                case '.':
                    return SpringCondition.Functional;
                default:
                    throw new Exception($"Cannot convert '{c}' to a SpringCondition");
            }
        }

        public static readonly Parser<SpringRow> rowParser =
            from springs in Parse.Chars(new char[] { '?', '#', '.' }).AtLeastOnce().Text().Token()
            from numbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.Char(','))
            select new SpringRow(springs.Select(CharToSpring).ToList(), numbers.Select(int.Parse).ToList());

        private static readonly Parser<List<SpringRow>> allRowsParser =
            from rows in rowParser.DelimitedBy(Parse.Char('\n'))
            select rows.ToList();
    }
}
