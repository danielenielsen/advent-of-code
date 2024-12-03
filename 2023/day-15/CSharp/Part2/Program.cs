namespace Part2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = File.ReadAllText(path);

            List<string> steps = input.Trim().Replace("\n", "").Split(",").ToList();

            List<List<Lens>> boxes = new List<List<Lens>>();
            for (int i = 0; i < 256; i++)
            {
                boxes.Add(new List<Lens>());
            }

            foreach (string step in steps)
            {
                if (step.Contains("-"))
                {
                    string label = step[0..^1];
                    int hash = Hashing(label);

                    List<Lens> box = boxes[hash];
                    List<Lens> newBox = new List<Lens>();

                    foreach (Lens lens in box)
                    {
                        if (lens.Label != label)
                        {
                            newBox.Add(lens);
                        }
                    }

                    boxes[hash] = newBox;
                }
                else
                {
                    var split = step.Split("=");
                    string label = split[0];
                    int focalLength = int.Parse(split[1]);

                    int hash = Hashing(label);
                    List<Lens> box = boxes[hash];

                    bool found = false;
                    foreach (Lens lens in box)
                    {
                        if (lens.Label == label)
                        {
                            lens.FocalLength = focalLength;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        box.Add(new Lens(label, focalLength));
                    }
                }
            }

            int result = 0;
            for (int i = 0; i < boxes.Count; i++)
            {
                List<Lens> box = boxes[i];

                for (int j = 0; j < box.Count; j++)
                {
                    result += (i + 1) * (j + 1) * box[j].FocalLength;
                }
            }

            Console.WriteLine($"Result: {result}");
        }

        private static int Hashing(string text)
        {
            int hash = 0;

            foreach (char c in text)
            {
                hash += c;
                hash *= 17;
                hash %= 256;
            }

            return hash;
        }
    }

    class Lens
    {
        public string Label;
        public int FocalLength;

        public Lens(string label, int focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }
    }
}
