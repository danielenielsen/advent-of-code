namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Machine> machines = Machine.ParseInput(input);
            int minCoins = machines.Select(x => x.FindMinCoins()).Sum();

            Console.WriteLine($"Result: {minCoins}");
        }
    }

    class Machine
    {
        public int ButtonAX;
        public int ButtonAY;
        public int ButtonBX;
        public int ButtonBY;
        public int PrizeX;
        public int PrizeY;

        private Machine(int buttonAX, int buttonAY, int buttonBX, int buttonBY, int prizeX, int prizeY)
        {
            ButtonAX = buttonAX;
            ButtonAY = buttonAY;
            ButtonBX = buttonBX;
            ButtonBY = buttonBY;
            PrizeX = prizeX;
            PrizeY = prizeY;
        }

        public int FindMinCoins()
        {
            int minA = 100;
            int minB = 100;
            bool found = false;

            for (int a = 0; a < 101; a++)
            {
                if (a * ButtonAX > PrizeX || a * ButtonAY > PrizeY)
                {
                    break;
                }

                for (int b = 0; b < 101; b++)
                {
                    if (a * ButtonAX + b * ButtonBX > PrizeX || a * ButtonAY + b * ButtonBY > PrizeY)
                    {
                        break;
                    }

                    if (a * ButtonAX + b * ButtonBX == PrizeX && a * ButtonAY + b * ButtonBY == PrizeY)
                    {
                        if (minA * 3 + minB > a * 3 + b)
                        {
                            found = true;
                            minA = a;
                            minB = b;
                        }
                    }
                }
            }

            if (!found)
            {
                return 0;
            }

            return minA * 3 + minB;
        }

        public static List<Machine> ParseInput(string input)
        {
            List<Machine> machines = [];

            string[] splits = input.Split("\n\n");

            foreach (string split in splits)
            {
                string[] lines = split.Split("\n");

                lines[0] = lines[0].Split(": ")[1];
                lines[1] = lines[1].Split(": ")[1];
                lines[2] = lines[2].Split(": ")[1];

                string[] buttonASplit = lines[0].Split(", ");
                string[] buttonBSplit = lines[1].Split(", ");
                string[] prizeSplit = lines[2].Split(", ");

                buttonASplit[0] = buttonASplit[0].Split("+")[1];
                buttonASplit[1] = buttonASplit[1].Split("+")[1];

                buttonBSplit[0] = buttonBSplit[0].Split("+")[1];
                buttonBSplit[1] = buttonBSplit[1].Split("+")[1];

                prizeSplit[0] = prizeSplit[0].Split("=")[1];
                prizeSplit[1] = prizeSplit[1].Split("=")[1];

                Machine machine = new Machine(
                    int.Parse(buttonASplit[0]),
                    int.Parse(buttonASplit[1]),
                    int.Parse(buttonBSplit[0]),
                    int.Parse(buttonBSplit[1]),
                    int.Parse(prizeSplit[0]),
                    int.Parse(prizeSplit[1])
                );

                machines.Add(machine);
            }

            return machines;
        }

        public override string ToString()
        {
            var res = "";

            res += $"Button A: X+{ButtonAX}, Y+{ButtonAY}\n";
            res += $"Button B: X+{ButtonBX}, Y+{ButtonBY}\n";
            res += $"Prize: X={PrizeX}, Y={PrizeY}";

            return res;
        }
    }
}
