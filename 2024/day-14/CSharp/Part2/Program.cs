﻿namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            List<Robot> robots = ParseInput(input);

            int count = 0;
            while (true)
            {
                List<Robot> topLeft = robots.FindAll(x => x.IsTopLeftQuadrant());
                //int topRight = robots.Where(x => x.IsTopRightQuadrant()).Count();
                //int bottomLeft = robots.Where(x => x.IsBottomLeftQuadrant()).Count();
                //int bottomRight = robots.Where(x => x.IsBottomRightQuadrant()).Count();

                //bool similar = topLeft == topRight && bottomLeft == bottomRight;



                if (true)
                {
                    Print(robots);
                    Console.WriteLine(count);
                    Thread.Sleep(1000);
                }
                robots.ForEach(r => r.Step(1));
                count++;
            }

            //Console.WriteLine($"Result: {topLeft * topRight * bottomLeft * bottomRight}");
        }

        static void Print(List<Robot> robots)
        {
            Console.Clear();

            for (int y = 0; y < 103; y++)
            {
                for (int x = 0; x < 101; x++)
                {
                    int robotCount = robots.Count(r => r.X == x && r.Y == y);

                    if (robotCount != 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }

                    Console.Write(".");
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine();
            }
        }

        static List<Robot> ParseInput(string input)
        {
            List<Robot> robots = new List<Robot>();
            string[] lines = input.Split("\n");

            foreach (string line in lines)
            {
                string[] split = line.Split(" ");

                split[0] = split[0].Split("=")[1];
                split[1] = split[1].Split("=")[1];

                string[] positionSplit = split[0].Split(",");
                string[] velocitySplit = split[1].Split(",");

                robots.Add(new Robot(
                    int.Parse(positionSplit[0]),
                    int.Parse(positionSplit[1]),
                    int.Parse(velocitySplit[0]),
                    int.Parse(velocitySplit[1])
                ));
            }

            return robots;
        }
    }

    class Robot(int x, int y, int velocityX, int velocityY)
    {
        public int X = x;
        public int Y = y;

        public int VelocityX = velocityX;
        public int VelocityY = velocityY;

        public void Step(int times)
        {
            foreach (var _ in Enumerable.Range(0, 100))
            {
                X += VelocityX;
                Y += VelocityY;

                if (X < 0)
                {
                    X += 101;
                }

                if (X >= 101)
                {
                    X -= 101;
                }

                if (Y < 0)
                {
                    Y += 103;
                }

                if (Y >= 103)
                {
                    Y -= 103;
                }
            }
        }

        public bool IsTopLeftQuadrant()
        {
            if (X >= 50)
            {
                return false;
            }

            if (Y >= 51)
            {
                return false;
            }

            return true;
        }

        public bool IsTopRightQuadrant()
        {
            if (X < 51)
            {
                return false;
            }

            if (Y >= 51)
            {
                return false;
            }

            return true;
        }

        public bool IsBottomLeftQuadrant()
        {
            if (X >= 50)
            {
                return false;
            }

            if (Y < 52)
            {
                return false;
            }

            return true;
        }

        public bool IsBottomRightQuadrant()
        {
            if (X < 51)
            {
                return false;
            }

            if (Y < 52)
            {
                return false;
            }

            return true;
        }
    }
}