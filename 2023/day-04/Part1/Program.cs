using Sprache;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);
            List<Card> cards = InputParser.ParseCards(input);
            List<int> numbers = cards.Select(x => x.GetPoints()).ToList();
            int result = cards.Select(x => x.GetPoints()).Sum();
        }
    }

    public class Card
    {
        public int ID;
        public List<int> WinningNumbers;
        public List<int> Numbers;

        public Card(int id, List<int> winningNumbers, List<int> numbers)
        {
            ID = id;
            WinningNumbers = winningNumbers;
            Numbers = numbers;
        }

        public int GetPoints()
        {
            int amountOfWinningNumbers = WinningNumbers.Count - WinningNumbers.Except(Numbers).Count() - 1;

            if (amountOfWinningNumbers < 0)
            {
                return 0;
            }

            return (int)Math.Pow(2, amountOfWinningNumbers);
        }
    }

    public static class InputParser
    {
        public static List<Card> ParseCards(string input)
        {
            return input.Split("\n").Where(x => x != "").Select(parseCard.Parse).ToList();
        }

        public static readonly Parser<Card> parseCard =
            from card in Parse.String("Card").Token()
            from id in Parse.Digit.AtLeastOnce().Text()
            from colon in Parse.Char(':').Token()
            from winningNumbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            from line in Parse.Char('|').Token()
            from numbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.WhiteSpace.AtLeastOnce())
            select new Card(int.Parse(id), winningNumbers.Select(int.Parse).ToList(), numbers.Select(int.Parse).ToList());
    }
}
