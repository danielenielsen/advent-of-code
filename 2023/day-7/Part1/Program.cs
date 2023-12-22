using Sprache;

namespace Part1
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

            var groups = cards.GroupBy(x => x);

            if (groups.Any(x => x.Count() == 5))
            {
                return HandType.FiveOfKind;
            }

            if (groups.Any(x => x.Count() == 4))
            {
                return HandType.FourOfKind;
            }

            if (groups.Any(x => x.Count() == 3) && groups.Any(x => x.Count() == 2))
            {
                return HandType.FullHouse;
            }

            if (groups.Any(x => x.Count() == 3))
            {
                return HandType.ThreeOfKind;
            }

            if (groups.Where(x => x.Count() == 2).Count() == 2)
            {
                return HandType.TwoPair;
            }

            if (groups.Any(x => x.Count() == 2))
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
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        T = 9,
        J = 10,
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
