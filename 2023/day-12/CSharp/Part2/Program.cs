using Sprache;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);
            List<SpringRow> rows = InputParser.ParseInput(input);
            rows.ForEach(SimplifyRow);

            // ?????????? 1,1,2
            // ???....??? 1,1,2

            //List<SpringCondition> springs = new List<SpringCondition>();
            //for (int i = 0; i < 45; i++)
            //{
            //    springs.Add(SpringCondition.Unknown);
            //}

            //List<int> logs = new List<int>
            //{
            //    1,
            //    1,
            //    2
            //};

            //logs = logs.Concat(logs).Concat(logs).Concat(logs).Concat(logs).ToList();

            // ?.?.?.?.???.?.??.?? 1,1

            //SpringRow row = InputParser.rowParser.Parse(".????? 1,3");
            //SpringRow row = InputParser.rowParser.Parse("?????????? 1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("???.#????#.? 1,1,1,1");
            //SpringRow row = InputParser.rowParser.Parse("????????????????????? 1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("???????????????????????????????? 1,1,2,1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("??????????????????????????????????????????? 1,1,2,1,1,2,1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("?????????????????????????????????????????????????????? 1,1,2,1,1,2,1,1,2,1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("?????????????????????????...?????????????????????????? 1,1,2,1,1,2,1,1,2,1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("?#????????????##????#????????????##????#????????????##????#????????????##????#??? 1,1,10,1,1,10,1,1,10,1,1,10,1,1,10");
            //SpringRow row = InputParser.rowParser.Parse("?#????????????##????#????????????##????#????????????##????#????????????##????#????????????# 1,1,10,1,1,10,1,1,10,1,1,10,1,1,10");
            SpringRow row = InputParser.rowParser.Parse("?#????????????##????#????????????##????#????????????##????#????????????##????#????????????##?? 1,1,10,1,1,10,1,1,10,1,1,10,1,1,10");
            //SpringRow row = InputParser.rowParser.Parse("...??????????...##??? 1,1,2,1,1,2");
            //SpringRow row = InputParser.rowParser.Parse("?#????????????##?? 1,1,10");

            //SpringRow row = new SpringRow(springs, logs);
            //SpringRow row = rows.ElementAt(11);
            //int test = CountCombinations(row2);
            //Console.WriteLine(test);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long result = CountCombinationsSliding(row);
            //long result = rows.Select(CountCombinations).Sum();
            stopwatch.Stop();

            Console.WriteLine($"Elapsed seconds: {stopwatch.ElapsedMilliseconds / 1000d}");
            Console.WriteLine($"Result: {result}");

            //SpringRow test = InputParser.rowParser.Parse("#??????????# 3,1,1,2");
            //SimplifyRow(test);

            //long result = 0;
            //int index = 0;

            //Stopwatch outer = new Stopwatch();
            //outer.Start();
            //foreach (SpringRow row in rows)
            //{
            //    Console.Write($"Starting row {index + 1}... ");
            //    Stopwatch inner = new Stopwatch();
            //    inner.Start();
            //    long combinations = CountCombinationsSliding(row);
            //    inner.Stop();
            //    Console.WriteLine($"Finished in {inner.ElapsedMilliseconds / 1000d} seconds with {combinations} combinations");

            //    result += combinations;
            //    index++;
            //}
            //outer.Stop();

            //Console.WriteLine($"Elapsed seconds: {outer.ElapsedMilliseconds / 1000d}");
            //Console.WriteLine($"Result: {result}");
        }

        private static void SimplifyRow(SpringRow row)
        {
            if (row.Springs.First() == SpringCondition.Damaged)
            {
                for (int i = 0; i < row.SpringLogs.First(); i++)
                {
                    row.Springs[i] = SpringCondition.Damaged;

                    if (i + 1 == row.SpringLogs.First())
                    {
                        row.Springs[i + 1] = SpringCondition.Functional;
                    }
                }
            }

            if (row.Springs.Last() == SpringCondition.Damaged)
            {
                for (int i = 0; i < row.SpringLogs.Last(); i++)
                {
                    int index = row.Springs.Count() - 1 - i;
                    row.Springs[index] = SpringCondition.Damaged;

                    if (i + 1 == row.SpringLogs.Last())
                    {
                        row.Springs[index - i] = SpringCondition.Functional;
                    }
                }
            }
        }

        private static long CountCombinationsSliding(SpringRow row)
        {
            SpringCondition[] original = [.. row.Springs];
            int[] groups = [.. row.SpringLogs];
            int[] groupPlacements = new int[groups.Length];
            int damagedNeededToBeOverwritten = original.Count(x => x == SpringCondition.Damaged);
            //(bool[,] validArray, int[,] countArray) = GeneratePreCompute(original, groups);

            int current = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                groupPlacements[i] = current;
                current += groups[i] + 1;
            }

            long total = 0;


            //(bool[] validGroups, int damageCount) = GetValidGroupsArray(original, groups, groupPlacements);
            //bool valid = !validGroups.Any(x => !x);

            //if (valid && damagedNeededToBeOverwritten != damageCount)
            //{
            //    valid = false;
            //}


            //if (valid)
            //if (!AnyInvalidPlacementForGroup(original, groups, groupPlacements, damagedNeededToBeOverwritten))
            ////if (ValidGroupsUsingDict(groups, groupPlacements, validArray, countArray, damagedNeededToBeOverwritten))
            //{
            //    total += 1;
            //}

            //while (true)
            //{




            //    bool anyMove = false;
            //    for (int i = groups.Length - 1; i >= 0; i--)
            //    {
            //        if (groupPlacements[i] + groups[i] < original.Length)
            //        {
            //            if (i + 1 < groups.Length && groupPlacements[i] + groups[i] + 1 >= groupPlacements[i + 1])
            //            {
            //                continue;
            //            }

            //            groupPlacements[i] += 1;
            //            anyMove = true;

            //            for (int j = i + 1; j < groups.Length; j++)
            //            {
            //                groupPlacements[j] = groupPlacements[j - 1] + groups[j - 1] + 1;
            //            }


            //            break;
            //        }
            //    }

            //    if (!anyMove)
            //    {
            //        break;
            //    }

            //    if (AnyInvalidPlacementForGroup(original, groups, groupPlacements, damagedNeededToBeOverwritten))
            //    //if (!ValidGroupsUsingDict(groups, groupPlacements, validArray, countArray, damagedNeededToBeOverwritten))
            //    {
            //        continue;
            //    }

            //    total += 1;
            //}

            //LoopEnd:

            while (true)
            {
                (int firstInvalidIndex, int damagedCount) = GetFirstInvalidGroup(original, groups, groupPlacements);

                // If no invalid groups
                if (firstInvalidIndex == -1)
                {
                    // If all damaged are overlapped by groups
                    if (damagedCount == damagedNeededToBeOverwritten)
                    {
                        total += 1;
                    }

                    // Move last element one to the right
                    bool anyMove = false;
                    for (int i = groups.Length - 1; i >= 0; i--)
                    {
                        if (groupPlacements[i] + groups[i] < original.Length)
                        {
                            if (i + 1 < groups.Length && groupPlacements[i] + groups[i] + 1 >= groupPlacements[i + 1])
                            {
                                continue;
                            }

                            groupPlacements[i] += 1;
                            anyMove = true;

                            for (int j = i + 1; j < groups.Length; j++)
                            {
                                groupPlacements[j] = groupPlacements[j - 1] + groups[j - 1] + 1;
                            }


                            break;
                        }
                    }

                    if (!anyMove)
                    {
                        break;
                    }

                    continue;
                }

                for (int i = firstInvalidIndex; i < groups.Length; i++)
                {
                    groupPlacements[i] += 1;
                }

                int lastIndex = groups.Length - 1;
                if (groupPlacements[lastIndex] + groups[lastIndex] > original.Length)
                {
                    break;
                }
            }

            return total;
        }

        private static void GenerateLookupInformation(SpringCondition[] original, int[] groups)
        {

        }


        private static bool ValidGroupsUsingDict(int[] groups, int[] groupPlacements, bool[,] validArray, int[,] countArray, int damagedNeededToBeOverwritten)
        {
            bool valid = true;
            int totalDamaged = 0;

            for (int i = 0; i < groups.Length; i++)
            {
                int start = groupPlacements[i];
                int length = groups[i];

                //(bool validGroup, int damagedCount) = preComputedGroupValues[(start, length)];


                valid = valid && validArray[start, length];
                totalDamaged += countArray[start, length];
            }

            if (damagedNeededToBeOverwritten != totalDamaged)
            {
                return false;
            }

            return valid;

        }

        private static (bool[,], int[,]) GeneratePreCompute(SpringCondition[] original, int[] groups)
        {
            int maxGroupSize = groups.Max() + 1;
            //Dictionary<(int, int), (bool, int)> preComputedGroupValues = new Dictionary<(int, int), (bool, int)>();

            bool[,] validArray = new bool[original.Length, maxGroupSize];
            int[,] countArray = new int[original.Length, maxGroupSize];

            for (int i = 0; i < original.Length; i++)
            {
                for (int j = 1; j < maxGroupSize; j++)
                {
                    bool valid = true;
                    int damagedCount = 0;

                    for (int k = 0; k < j; k++)
                    {
                        int index = i + k;

                        if (index >= original.Length)
                        {
                            valid = false;
                            continue;
                        }

                        SpringCondition spring = original[index];

                        if (spring == SpringCondition.Damaged)
                        {
                            damagedCount++;
                        }

                        if (spring == SpringCondition.Functional)
                        {
                            valid = false;
                        }
                    }

                    //preComputedGroupValues[(i, distinctGroupSizes[j])] = (valid, damagedCount);
                    validArray[i, j] = valid;
                    countArray[i, j] = damagedCount;
                }
            }

            return (validArray, countArray);
        }

        private static (int, int) GetFirstInvalidGroup(SpringCondition[] original, int[] groups, int[] groupPlacements)
        {
            //bool[] validGroups = new bool[groups.Length];

            //for (int i = 0; i < validGroups.Length; i++)
            //{
            //    validGroups[i] = true;
            //}

            int firstInvalid = -1;
            int damagedCount = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i]; j++)
                {
                    int index = groupPlacements[i] + j;

                    // If group end + 1 is a '#', then invalid group

                    if (original[index] == SpringCondition.Functional)
                    {
                        firstInvalid = i;
                        break;
                    }
                    else if (original[index] == SpringCondition.Damaged)
                    {
                        damagedCount += 1;
                    }
                }
            }

            return (firstInvalid, damagedCount);
        }

        private static bool AnyInvalidPlacementForGroup(SpringCondition[] original, int[] groups, int[] groupPlacements, int damagedNeededToBeOverwritten)
        {
            //SpringCondition[] copy = original.ToArray();

            int damagedOverwrittenCount = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i]; j++)
                {
                    int index = groupPlacements[i] + j;

                    if (original[index] == SpringCondition.Functional)
                    {
                        return true;
                    }

                    if (original[index] == SpringCondition.Damaged)
                    {
                        damagedOverwrittenCount += 1;
                    }
                }
            }

            if (damagedOverwrittenCount != damagedNeededToBeOverwritten)
            {
                return true;
            }

            return false;


            //bool[] insideGroup = new bool[original.Length];

            //for (int i = 0; i < groups.Length; i++)
            //{
            //    for (int j = 0; j < groups[i]; j++)
            //    {
            //        int index = groupPlacements[i];
            //        insideGroup[index + j] = true;
            //    }
            //}


            //for (int i = 0; i < original.Length; i++)
            //{
            //    bool isGroup = insideGroup[i];
            //    SpringCondition spring = original[i];
            //    if (!isGroup && spring == SpringCondition.Damaged)
            //    {
            //        return true;
            //    }

            //    if (isGroup && spring == SpringCondition.Functional)
            //    {
            //        return true;
            //    }
            //}




            //for (int i = 0; i < groups.Length; i++)
            //{
            //    for (int j = 0; j < groups[i]; j++)
            //    {
            //        int index = groupPlacements[i];
            //        SpringCondition spring = original[index + j];

            //        if (spring == SpringCondition.Functional)
            //        {
            //            return true;
            //        }
            //    }
            //}
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

        public SpringRow(List<SpringCondition> springs, List<int> springLogs)
        {
            Springs = springs;
            SpringLogs = springLogs;
        }
    }

    public static class InputParser
    {
        public static List<SpringRow> ParseInput(string input)
        {
            List<SpringRow> rows = allRowsParser.Parse(input);
            List<SpringRow> expandedRows = new List<SpringRow>();

            foreach (SpringRow row in rows)
            {
                List<SpringCondition> partToAdd = [SpringCondition.Unknown, .. row.Springs];

                List<SpringCondition> newSprings = row.Springs.Concat(partToAdd).Concat(partToAdd).Concat(partToAdd).Concat(partToAdd).ToList();
                List<int> newLogs = row.SpringLogs.Concat(row.SpringLogs).Concat(row.SpringLogs).Concat(row.SpringLogs).Concat(row.SpringLogs).ToList();

                //List<SpringCondition> newSprings = row.Springs.Concat(partToAdd).Concat(partToAdd).ToList();
                //List<int> newLogs = row.SpringLogs.Concat(row.SpringLogs).Concat(row.SpringLogs).ToList();

                //List<SpringCondition> newSprings = row.Springs;
                //List<int> newLogs = row.SpringLogs;

                expandedRows.Add(new SpringRow(newSprings, newLogs));
            }

            return expandedRows;
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
