using System.Collections.Generic;

namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path).Replace("\r", "").Trim();
            Board board = Board.ParseInput(input);

            int result = board.CalculateTotalCost();
            Console.WriteLine($"Result: {result}");
        }
    }

    class Board
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;

        private Board(int width, int height, Tile[,] tiles)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        public int CalculateTotalCost()
        {
            int totalCost = 0;

            foreach ((int x, int y, Tile tile) in Iter())
            {
                if (tile.Visited)
                {
                    continue;
                }

                totalCost += CalculateRegionCost(x, y);
            }

            return totalCost;
        }

        private int CalculateRegionCost(int x, int y)
        {
            char label = Tiles[x, y].Label;

            int area = 0;

            List<(int x, int y)> queue = [(x, y)];

            List<(int x, int y, Direction dir)> perimeters = [];

            while (queue.Count > 0)
            {
                (int currentX, int currentY) = queue[0];
                queue.RemoveAt(0);

                Tile current = Tiles[currentX, currentY];

                if (current.Visited)
                {
                    continue;
                }

                current.Visited = true;
                area++;

                if (currentX == 0)
                {
                    perimeters.Add((currentX, currentY, Direction.West));
                }

                if (currentX == Width - 1)
                {
                    perimeters.Add((currentX, currentY, Direction.East));
                }

                if (currentY == 0)
                {
                    perimeters.Add((currentX, currentY, Direction.North));
                }

                if (currentY == Height - 1)
                {
                    perimeters.Add((currentX, currentY, Direction.South));
                }

                foreach ((int nx, int ny, Direction dir) in GetNeighbors(currentX, currentY))
                {
                    Tile neighbor = Tiles[nx, ny];

                    if (neighbor.Label != label)
                    {
                        perimeters.Add((currentX, currentY, dir));
                        continue;
                    }

                    queue.Add((nx, ny));
                }
            }

            int sides = 0;
            while (perimeters.Count > 0)
            {
                (int x, int y, Direction dir) perimeter = perimeters[0];
                sides++;

                var sameSides = FindSameSide(perimeters, perimeter);

                foreach (var side in sameSides)
                {
                    perimeters.Remove(side);
                }
            }

            return area * sides;
        }

        private List<(int x, int y, Direction dir)> FindSameSide(List<(int x, int y, Direction dir)> perimeters, (int x, int y, Direction dir) start)
        {
            List<(int x, int y, Direction dir)> sameSide = [start];

            bool changes = true;
            while (changes)
            {
                changes = false;

                List<(int x, int y, Direction dir)> sidesToAdd = [];
                foreach (var side in sameSide)
                {
                    foreach (var diff in GetDirectionDiffs(side.dir))
                    {
                        int perimeterIndex = perimeters.FindIndex(p => p.x == side.x + diff.x && p.y == side.y + diff.y && p.dir == side.dir);
                        if (perimeterIndex != -1 && !sameSide.Contains(perimeters[perimeterIndex]))
                        {
                            sidesToAdd.Add(perimeters[perimeterIndex]);
                            perimeters.RemoveAt(perimeterIndex);

                            changes = true;
                        }
                    }
                }

                sameSide.AddRange(sidesToAdd);
            }

            return sameSide;
        }

        private IEnumerable<(int x, int y)> GetDirectionDiffs(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return [(-1, 0), (1, 0)];
                case Direction.South:
                    return [(-1, 0), (1, 0)];
                case Direction.East:
                    return [(0, -1), (0, 1)];
                case Direction.West:
                    return [(0, -1), (0, 1)];
                default:
                    throw new Exception("Invalid direction");
            }
        }

        private IEnumerable<(int x, int y, Direction dir)> GetNeighbors(int x, int y)
        {
            if (x > 0)
            {
                yield return (x - 1, y, Direction.West);
            }
            if (x < Width - 1)
            {
                yield return (x + 1, y, Direction.East);
            }
            if (y > 0)
            {
                yield return (x, y - 1, Direction.North);
            }
            if (y < Height - 1)
            {
                yield return (x, y + 1, Direction.South);
            }
        }

        private IEnumerable<(int x, int y, Tile tile)> Iter()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return (x, y, Tiles[x, y]);
                }
            }
        }

        public static Board ParseInput(string input)
        {
            string[] lines = input.Split("\n");
            int width = lines[0].Length;
            int height = lines.Length;

            Tile[,] tiles = new Tile[width, height];
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile(line[x]);
                }
            }

            return new Board(width, height, tiles);
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Tiles[x, y].Label);
                }
                Console.WriteLine();
            }
        }
    }

    class Tile(char label)
    {
        public char Label = label;
        public bool Visited = false;
    }

    enum Direction
    {
        North,
        South,
        East,
        West,
    }
}
