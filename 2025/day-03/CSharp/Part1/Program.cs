namespace Part1;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Bank> banks = ParseInput(input);
        int res = Solve(banks);
        Console.WriteLine($"Result: {res}");
    }

    private static int Solve(IEnumerable<Bank> banks)
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

    public int GetMaxJoltage()
    {
        int firstJoltage = 0;
        int secondJoltage = 0;

        for (int i = 0; i < Batteries.Count; i++)
        {
            Battery battery = Batteries[i];
            
            if (battery.Joltage > firstJoltage && i < Batteries.Count - 1)
            {
                firstJoltage = battery.Joltage;
                secondJoltage = Batteries[i + 1].Joltage;
                continue;
            }

            if (battery.Joltage > secondJoltage)
            {
                secondJoltage = battery.Joltage;
            }
        }
        
        return firstJoltage * 10 + secondJoltage;
    }
}
