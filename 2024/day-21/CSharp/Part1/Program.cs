namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<string> codes = ParseInput(input);

            GenerateNumericKeypadSequences(codes[0]).ForEach(Console.WriteLine);
        }

        private static List<string> ParseInput(string input)
        {
            return input.Split("\n").ToList();
        }

        private static List<string> GenerateNumericKeypadSequences(string code)
        {
            List<string> current = [];
            char from = 'A';

            foreach (char toChar in code)
            {
                (int x, int y) fromPos = GetNumberPosition(from);

                List<string> sequences = GenerateSequencesToNum(toChar, fromPos);

                if (current.Count == 0)
                {
                    current = sequences;
                }
                else
                {
                    List<string> newCurrent = [];
                    foreach (string cur in current)
                    {
                        foreach (string seq in sequences)
                        {
                            newCurrent.Add(cur + seq);
                        }
                    }
                    current = newCurrent;
                }

                from = toChar;
            }

            return current;
        }

        private static List<string> GenerateSequencesToNum(char to, (int x, int y) pos)
        {
            (int x, int y) numPos = GetNumberPosition(to);

            if (numPos == pos)
            {
                return ["A"];
            }

            if (pos == (0, 0))
            {
                return [];
            }

            List<string> res = [];

            if (numPos.x > pos.x)
            {
                List<string> sequences = GenerateSequencesToNum(to, (pos.x + 1, pos.y)).ConvertAll(s => ">" + s);
                res.AddRange(sequences);
            }

            if (numPos.x < pos.x)
            {
                List<string> sequences = GenerateSequencesToNum(to, (pos.x - 1, pos.y)).ConvertAll(s => "<" + s);
                res.AddRange(sequences);
            }

            if (numPos.y > pos.y)
            {
                List<string> sequences = GenerateSequencesToNum(to, (pos.x, pos.y + 1)).ConvertAll(s => "^" + s);
                res.AddRange(sequences);
            }

            if (numPos.y < pos.y)
            {
                List<string> sequences = GenerateSequencesToNum(to, (pos.x, pos.y - 1)).ConvertAll(s => "v" + s);
                res.AddRange(sequences);
            }

            return res;
        }

        private static (int x, int y) GetNumberPosition(char num)
        {
            return num switch
            {
                '0' => (1, 0),
                'A' => (2, 0),
                '1' => (0, 1),
                '2' => (1, 1),
                '3' => (2, 1),
                '4' => (0, 2),
                '5' => (1, 2),
                '6' => (2, 2),
                '7' => (0, 3),
                '8' => (1, 3),
                '9' => (2, 3),
                _ => throw new Exception($"Invalid number '{num}'."),
            };
        }

        private static (int x, int y) GetDirectionPosition(char dir)
        {
            return dir switch
            {
                '<' => (0, 0),
                'v' => (1, 0),
                '>' => (2, 0),
                '^' => (1, 1),
                'A' => (2, 1),
                _ => throw new Exception($"Invalid direction '{dir}'."),
            };
        }
    }
}
