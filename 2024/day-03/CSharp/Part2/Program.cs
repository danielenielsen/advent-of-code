namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Trim().Replace("\r", "").Replace("\n", "");

            bool enabled = true;
            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (i + 6 < input.Length && input[i..(i + 7)] == "don't()")
                {
                    enabled = false;
                }

                if (i + 3 < input.Length && input[i..(i + 4)] == "do()")
                {
                    enabled = true;
                }

                if (!enabled)
                {
                    continue;
                }

                if (i + 3 < input.Length && input[i..(i + 4)] != "mul(")
                {
                    continue;
                }

                int pos = input[(i + 4)..].IndexOf(')');

                if (pos == -1)
                {
                    break;
                }

                if (pos < 3)
                {
                    continue;
                }

                List<string> split = input[(i + 4)..(i + 4 + pos)].Split(',').ToList();
                if (split.Count != 2)
                {
                    continue;
                }

                if (split[0] != split[0].Trim())
                {
                    continue;
                }

                if (split[1] != split[1].Trim())
                {
                    continue;
                }

                if (!int.TryParse(split[0], out int x))
                {
                    continue;
                }

                if (!int.TryParse(split[1], out int y))
                {
                    continue;
                }

                result += x * y;
            }

            Console.WriteLine($"Result: {result}");
        }
    }
}