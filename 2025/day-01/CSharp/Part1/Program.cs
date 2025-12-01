namespace Part1;

internal class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<Instruction> instructions = ParseInput(input);
        //PrintInstructions(instructions);

        int result = Solve(instructions);
        Console.WriteLine($"Result: {result}");
    }

    private static int Solve(IEnumerable<Instruction> instructions)
    {
        int zeroCount = 0;
        int currentPosition = 50;

        foreach (Instruction instruction in instructions)
        {
            int stepChange = instruction.Direction == Direction.Left ? -1 : 1;

            for (int i = 0; i < instruction.Steps; i++)
            {
                currentPosition += stepChange;

                if (currentPosition < 0)
                {
                    currentPosition = 99;
                }

                if (currentPosition > 99)
                {
                    currentPosition = 0;
                }
            }

            if (currentPosition == 0)
            {
                zeroCount++;
            }
        }

        return zeroCount;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Instruction> ParseInput(string input)
    {
        string[] lines = input.Trim().Replace("\r", "").Split("\n");
        List<Instruction> res = new List<Instruction>(lines.Length);

        foreach (string line in lines)
        {
            Direction dir = line[0] switch
            {
                'L' => Direction.Left,
                'R' => Direction.Right,
                _ => throw new InvalidOperationException("Invalid direction"),
            };

            int steps = int.Parse(line[1..]);
            Instruction instruction = new Instruction(dir, steps);

            res.Add(instruction);
        }

        return res;
    }

    private static void PrintInstructions(IEnumerable<Instruction> instructions)
    {
        foreach (Instruction instruction in instructions)
        {
            Console.WriteLine(instruction.ToString());
        }
    }
}

internal enum Direction
{
    Left,
    Right,
}

internal static class DirectionExtensions
{
    internal static char ToChar(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => 'L',
            Direction.Right => 'R',
            _ => throw new InvalidOperationException("Invalid direction"),
        };
    }
}

internal readonly struct Instruction(Direction direction, int steps)
{
    public Direction Direction { get; } = direction;
    public int Steps { get; } = steps;

    public override string ToString()
    {
        return $"{Direction.ToChar()}{Steps}";
    }
}
