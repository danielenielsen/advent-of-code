namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../../input.txt";
            string input = System.IO.File.ReadAllText(path).Replace("\r", "").Trim();
            DiskMap diskMap = DiskMap.ParseInput(input);

            long checksum = diskMap.GetChecksum();
            Console.WriteLine($"Checksum: {checksum}");
        }
    }

    class DiskMap
    {
        public List<FileSystemItem?> Items;

        private DiskMap(List<FileSystemItem?> items)
        {
            Items = items;
        }

        public static DiskMap ParseInput(string input)
        {
            List<FileSystemItem?> items = new List<FileSystemItem?>();

            for (int i = 0; i < input.Length; i++)
            {
                string num = input[i].ToString();

                if (i % 2 == 0)
                {
                    int id = i / 2;
                    int length = int.Parse(num);

                    for (int j = 0; j < length; j++)
                    {
                        items.Add(new FileSystemItem(id));
                    }
                }
                else
                {
                    int length = int.Parse(num);

                    for (int j = 0; j < length; j++)
                    {
                        items.Add(null);
                    }
                }
            }

            return new DiskMap(items);
        }

        private List<FileSystemItem?> CompactFileSystem()
        {
            FileSystemItem?[] items = Items.ToArray();

            int i = 0;
            int j = items.Length - 1;

            while (true)
            {
                while (items[i] != null)
                {
                    i++;
                }

                while (items[j] == null)
                {
                    j--;
                }

                if (i > j)
                {
                    break;
                }

                FileSystemItem? tmp = items[i];
                items[i] = items[j];
                items[j] = tmp;
            }

            for (int m = 0; m < items.Length - 1; m++)
            {
                FileSystemItem? item1 = items[m];
                FileSystemItem? item2 = items[m + 1];

                if (item1 == null && item2 != null)
                {
                    throw new Exception("The file items were not properly compacted");
                }
            }

            return items.ToList();
        }

        public long GetChecksum()
        {
            List<FileSystemItem?> compactedItems = CompactFileSystem();

            long checksum = 0;
            for (int i = 0; i < compactedItems.Count; i++)
            {
                FileSystemItem? item = compactedItems[i];

                if (item == null)
                {
                    break;
                }

                checksum += i * item.ID;
            }

            return checksum;
        }
    }

    class FileSystemItem(int id)
    {
        public int ID = id;
    }
}
