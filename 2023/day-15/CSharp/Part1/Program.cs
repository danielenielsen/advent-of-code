namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);

            List<string> steps = input.Trim().Replace("\n", "").Split(',').ToList();

            int result = 0;
            foreach (string step in steps)
            {
                int hash = 0;
                foreach (char c in step)
                {
                    hash += c;
                    hash *= 17;
                    hash %= 256;
                }

                result += hash;
            }

            Console.WriteLine($"Result: {result}");
        }
    }
}
