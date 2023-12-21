using Sprache;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);
            List<Card> cards = InputParser.ParseCards(input);
            Dictionary<int, int> cardAmountDict = cards.ToDictionary(x => x.ID, x => 1);

            foreach (Card card in cards)
            {
                int amountOfCards = cardAmountDict[card.ID];
                int amountOfWinningNumbers = card.GetAmountOfWinningNumbers();

                for (int i = card.ID + 1; i <= card.ID + amountOfWinningNumbers; i++)
                {
                    cardAmountDict[i] += amountOfCards;
                }
            }

            int result = cardAmountDict.Values.Sum();
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

        public int GetAmountOfWinningNumbers()
        {
            return WinningNumbers.Count - WinningNumbers.Except(Numbers).Count();
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
