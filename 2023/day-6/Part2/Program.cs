using Sprache;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);
            TimeDistancePair pair = InputParser.ParseInput(input);
            long result = pair.GetBeatingTimes();
        }
    }

    public class TimeDistancePair
    {
        long Time;
        long Distance;

        public TimeDistancePair(long time, long distance)
        {
            Time = time;
            Distance = distance;
        }

        public long GetBeatingTimes()
        {
            long result = 0;

            for (long i = 0; i <= Time; i++)
            {
                long speed = i;
                long remainingTime = Time - i;

                if (speed * remainingTime > Distance)
                {
                    result += 1;
                }
            }

            return result;
        }
    }

    public static class InputParser
    {
        public static TimeDistancePair ParseInput(string input)
        {
            return raceParser.Parse(input);
        }

        private static readonly Parser<TimeDistancePair> raceParser =
            from title1 in Parse.String("Time:").Token()
            from times in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            from title2 in Parse.String("Distance:").Token()
            from distances in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            select new TimeDistancePair(long.Parse(times.Aggregate("", (acc, val) => acc + val)), long.Parse(distances.Aggregate("", (acc, val) => acc + val)));
    }
}
