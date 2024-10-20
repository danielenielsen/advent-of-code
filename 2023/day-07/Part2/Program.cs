using Sprache;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);
            IEnumerable<HandBidPair> handBidPairs = InputParser.ParseInput(input);
            IEnumerable<(HandBidPair, int)> handBidPairsWithRank = handBidPairs.SortByFunc((a, b) => CompareHands(a.Hand, b.Hand)).Select((val, index) => (val, index + 1));
            int result = handBidPairsWithRank.Aggregate(0, (acc, val) => acc + val.Item1.Bid * val.Item2);
        }

        private static HandType GetHandType(IEnumerable<Card> cards)
        {
            if (cards.Count() != 5)
            {
                throw new Exception($"The card collection should have 5 elements, it had {cards.Count()}");
            }

            if (cards.All(x => x == Card.J))
            {
                return HandType.FiveOfKind;
            }

            int numOfJokers = cards.Count(x => x == Card.J);
            List<Card> cardsWithoutJokers = cards.Where(x => x != Card.J).ToList();
            var groups = cardsWithoutJokers.GroupBy(x => x);

            int highest = groups.OrderByDescending(x => x.Count()).First().Count();

            if (highest + numOfJokers == 5)
            {
                return HandType.FiveOfKind;
            }

            if (highest + numOfJokers == 4)
            {
                return HandType.FourOfKind;
            }

            int nextHighest = groups.OrderByDescending(x => x.Count()).Skip(1).First().Count();

            if (highest + numOfJokers == 3 && nextHighest + numOfJokers - (3 - highest) == 2)
            {
                return HandType.FullHouse;
            }

            if (highest + numOfJokers == 3)
            {
                return HandType.ThreeOfKind;
            }

            if (highest + numOfJokers == 2 && nextHighest + numOfJokers - (2 - highest) == 2)
            {
                return HandType.TwoPair;
            }

            if (highest + numOfJokers == 2)
            {
                return HandType.OnePair;
            }

            return HandType.HighCard;
        }

        private static int CompareHands(IEnumerable<Card> a, IEnumerable<Card> b)
        {
            if (GetHandType(a) > GetHandType(b))
            {
                return 1;
            }

            if (GetHandType(a) < GetHandType(b))
            {
                return -1;
            }

            for (int i = 0; i < 5; i++)
            {
                if (a.ElementAt(i) > b.ElementAt(i))
                {
                    return 1;
                }

                if (a.ElementAt(i) < b.ElementAt(i))
                {
                    return -1;
                }
            }

            return 0;
        }
    }

    public enum Card
    {
        J = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        T = 10,
        Q = 11,
        K = 12,
        A = 13
    }

    public enum HandType
    {
        HighCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfKind = 4,
        FullHouse = 5,
        FourOfKind = 6,
        FiveOfKind = 7
    }

    public class HandBidPair
    {
        public IEnumerable<Card> Hand;
        public int Bid;

        public HandBidPair(IEnumerable<Card> hand, int bid)
        {
            Hand = hand;
            Bid = bid;
        }
    }

    public static class InputParser
    {
        public static IEnumerable<HandBidPair> ParseInput(string input)
        {
            return allHandParser.Parse(input);
        }

        private static Card GetCardFromChar(char c)
        {
            switch (c)
            {
                case '2':
                    return Card.Two;
                case '3':
                    return Card.Three;
                case '4':
                    return Card.Four;
                case '5':
                    return Card.Five;
                case '6':
                    return Card.Six;
                case '7':
                    return Card.Seven;
                case '8':
                    return Card.Eight;
                case '9':
                    return Card.Nine;
                case 'T':
                    return Card.T;
                case 'J':
                    return Card.J;
                case 'Q':
                    return Card.Q;
                case 'K':
                    return Card.K;
                case 'A':
                    return Card.A;
                default:
                    throw new Exception($"\"{c}\" is not a known card");
            }
        }

        private static readonly Parser<HandBidPair> handParser =
            from cards in Parse.LetterOrDigit.AtLeastOnce().Token()
            from bid in Parse.Digit.AtLeastOnce().Text()
            select new HandBidPair(cards.Select(GetCardFromChar), int.Parse(bid));

        private static readonly Parser<IEnumerable<HandBidPair>> allHandParser =
            handParser.DelimitedBy(Parse.Char('\n'));
    }

    public static class LinqExtensions
    {
        public static IEnumerable<T> SortByFunc<T>(this IEnumerable<T> source, Func<T, T, int> comparison)
        {
            List<T> result = source.ToList();
            result.Sort((a, b) => comparison(a, b));
            return result;
        }
    }
}
