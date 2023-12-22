using Sprache;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);
            RaceInformation raceInformation = InputParser.ParseInput(input);
            int result = raceInformation.TimeDistancePairs.Select(x => x.GetBeatingTimes()).Aggregate(1, (acc, val) => acc * val);
        }
    }

    public class TimeDistancePair
    {
        int Time;
        int Distance;

        public TimeDistancePair(int time, int distance)
        {
            Time = time;
            Distance = distance;
        }

        public int GetBeatingTimes()
        {
            int result = 0;

            for (int i = 0; i <= Time; i++)
            {
                int speed = i;
                int remainingTime = Time - i;

                if (speed * remainingTime > Distance)
                {
                    result += 1;
                }
            }

            return result;
        }
    }

    public class RaceInformation
    {
        public List<TimeDistancePair> TimeDistancePairs;

        public RaceInformation(IEnumerable<int> times, IEnumerable<int> distances)
        {
            TimeDistancePairs = times.Select((val, index) => new TimeDistancePair(val, distances.ElementAt(index))).ToList();
        }
    }

    public static class InputParser
    {
        public static RaceInformation ParseInput(string input)
        {
            return raceParser.Parse(input);
        }

        private static readonly Parser<RaceInformation> raceParser =
            from title1 in Parse.String("Time:").Token()
            from times in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            from title2 in Parse.String("Distance:").Token()
            from distances in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            select new RaceInformation(times.Select(int.Parse), distances.Select(int.Parse));
    }
}
