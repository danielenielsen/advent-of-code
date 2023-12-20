using Sprache;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);

            List<Game> games = GamesParser.ParseGames(input);
            List<int> powers = games.Select(x => x.GetMinValidValueColorsPower()).ToList();
            int result = powers.Sum();
        }
    }

    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public class CubeGroup
    {
        public int Number;
        public Color Color;

        public CubeGroup(int number, Color color)
        {
            Number = number;
            Color = color;
        }
    }

    public class Turn
    {
        public List<CubeGroup> CubeGroups;

        public Turn(IEnumerable<CubeGroup> cubeGroups)
        {
            CubeGroups = cubeGroups.ToList();
        }
    }

    public class Game
    {
        public int ID;
        public List<Turn> Turns;

        public Game(int id, IEnumerable<Turn> turns)
        {
            ID = id;
            Turns = turns.ToList();
        }

        public bool IsValid(int redNum, int greenNum, int blueNum)
        {
            foreach (Turn turn in Turns)
            {
                foreach (CubeGroup cubeGroup in turn.CubeGroups)
                {
                    if (cubeGroup.Color == Color.Red && cubeGroup.Number > redNum)
                    {
                        return false;
                    }

                    if (cubeGroup.Color == Color.Green && cubeGroup.Number > greenNum)
                    {
                        return false;
                    }

                    if (cubeGroup.Color == Color.Blue && cubeGroup.Number > blueNum)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetMinValidValueColorsPower()
        {
            int redNum = 0;
            int greenNum = 0;
            int blueNum = 0;

            foreach (Turn turn in Turns)
            {
                foreach (CubeGroup cubeGroup in turn.CubeGroups)
                {
                    if (cubeGroup.Color == Color.Red && cubeGroup.Number > redNum)
                    {
                        redNum = cubeGroup.Number;
                    }

                    if (cubeGroup.Color == Color.Green && cubeGroup.Number > greenNum)
                    {
                        greenNum = cubeGroup.Number;
                    }

                    if (cubeGroup.Color == Color.Blue && cubeGroup.Number > blueNum)
                    {
                        blueNum = cubeGroup.Number;
                    }
                }
            }

            return redNum * greenNum * blueNum;
        }
    }

    public static class GamesParser
    {
        public static List<Game> ParseGames(string input)
        {
            return gamesParser.Parse(input);
        }

        private static Color StringToColor(string colorString)
        {
            string colorStringLower = colorString.ToLower();

            switch (colorStringLower)
            {
                case "red":
                    return Color.Red;
                case "green":
                    return Color.Green;
                case "blue":
                    return Color.Blue;
                default:
                    throw new Exception($"Color string {colorString} did not match any values");
            }
        }

        private static readonly Parser<CubeGroup> cubeGroupParser =
            from number in Parse.Digit.AtLeastOnce().Text()
            from space in Parse.Char(' ')
            from color in Parse.Letter.AtLeastOnce().Text()
            select new CubeGroup(int.Parse(number), StringToColor(color));

        private static readonly Parser<Turn> turnParser =
            from cubeGroups in cubeGroupParser.DelimitedBy(Parse.String(", "))
            select new Turn(cubeGroups);

        private static readonly Parser<List<Turn>> turnsParser =
            from turns in turnParser.DelimitedBy(Parse.String("; "))
            select turns.ToList();

        private static readonly Parser<Game> gameParser =
            from gameText in Parse.String("Game")
            from space in Parse.Char(' ')
            from id in Parse.Digit.AtLeastOnce().Text()
            from colon in Parse.String(": ")
            from turns in turnsParser
            select new Game(int.Parse(id), turns);

        private static readonly Parser<List<Game>> gamesParser =
            from games in gameParser.DelimitedBy(Parse.Char('\n'))
            select games.ToList();
    }
}
