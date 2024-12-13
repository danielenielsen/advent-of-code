namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Machine> machines = Machine.ParseInput(input);

            long minCoins = machines.Select(x => x.FindMinCoins()).Sum();
            Console.WriteLine($"Result: {minCoins}");

            //Console.WriteLine(string.Join("\n\n", machines.Select(x => x.ToString())));
        }
    }

    class Machine
    {
        public int ButtonAX;
        public int ButtonAY;
        public int ButtonBX;
        public int ButtonBY;
        public long PrizeX;
        public long PrizeY;

        private Machine(int buttonAX, int buttonAY, int buttonBX, int buttonBY, long prizeX, long prizeY)
        {
            ButtonAX = buttonAX;
            ButtonAY = buttonAY;
            ButtonBX = buttonBX;
            ButtonBY = buttonBY;
            PrizeX = prizeX;
            PrizeY = prizeY;
        }

        public long FindMinCoins()
        {
            long minA = 0;
            long minB = 0;
            bool found = false;

            int a = 0;
            while (true)
            {
                if (a * ButtonAX > PrizeX || a * ButtonAY > PrizeY)
                {
                    break;
                }

                long remainingPrizeX = PrizeX - a * ButtonAX;

                if (remainingPrizeX % ButtonBX != 0)
                {
                    a++;
                    continue;
                }

                long b = remainingPrizeX / ButtonBX;

                if (a * ButtonAX + b * ButtonBX == PrizeX && a * ButtonAY + b * ButtonBY == PrizeY)
                {
                    if (!found)
                    {
                        found = true;
                        minA = a;
                        minB = b;
                    }
                    else if (minA * 3 + minB > a * 3 + b)
                    {
                        minA = a;
                        minB = b;
                    }
                }

                a++;
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
                    int.Parse(prizeSplit[0]) + 10000000000000,
                    int.Parse(prizeSplit[1]) + 10000000000000
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
