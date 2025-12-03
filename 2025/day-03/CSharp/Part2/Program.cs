namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Bank> banks = ParseInput(input);
        long res = Solve(banks);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(IEnumerable<Bank> banks)
    {
        return banks.Select(x => x.GetMaxJoltage()).Sum();
    }
    
    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Bank> ParseInput(string input)
    {
        string[] lines = input.Trim().Replace("\r", "").Split("\n");
        List<Bank> res = [];

        foreach (string line in lines)
        {
            List<Battery> batteries = [];

            foreach (char c in line)
            {
                int joltage = int.Parse(c.ToString());
                batteries.Add(new Battery(joltage));
            }
            
            res.Add(new Bank(batteries));
        }
        
        return res;
    }
}

internal class Battery(int joltage)
{
    public int Joltage { get; } = CheckJoltage(joltage);

    private static int CheckJoltage(int joltage)
    {
        if (joltage is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(joltage));
        }
        
        return joltage;
    }
}

internal class Bank(List<Battery> batteries)
{
    public List<Battery> Batteries { get; } = batteries;

    public long GetMaxJoltage()
    {
        int[] joltageIndexes = Enumerable.Repeat(-1, 12).ToArray();

        for (int i = 0; i < joltageIndexes.Length; i++)
        {
            int previousIndex = i > 0 ? joltageIndexes[i - 1] : -1;
            
            for (int j = previousIndex + 1; j < Batteries.Count - (joltageIndexes.Length - i - 1); j++)
            {
                if (joltageIndexes[i] == -1)
                {
                    joltageIndexes[i] = j;
                    continue;
                }
                
                if (Batteries[j].Joltage > Batteries[joltageIndexes[i]].Joltage)
                {
                    joltageIndexes[i] = j;
                }
            }
        }
        
        return joltageIndexes
            .Select(x => Batteries[x])
            .Reverse()
            .Select((val, idx) => (long)Math.Pow(10, idx) * val.Joltage)
            .Sum();
    }
}
