namespace Part2
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

        private static int FindMatchingEmptySpace(FileSystemItem?[] items, int length, int stopIdx)
        {
            int i = 0;
            int count = 0;
            while (i < stopIdx)
            {
                if (items[i] != null)
                {
                    count = 0;
                    i++;
                    continue;
                }

                count++;

                if (count == length)
                {
                    return i - count + 1;
                }

                i++;
            }

            return -1;
        }

        private static (int idx, int count) FindFileFromEnd(FileSystemItem?[] items, int startIdx)
        {
            int i = startIdx;
            while (i >= 0 && items[i] == null)
            {
                i--;
            }
            
            if (i < 0)
            {
                return (-1, -1);
            }

            int id = items[i]!.ID;
            int count = 0;
            while (i >= 0 && items[i] != null && items[i]!.ID == id)
            {
                count++;
                i--;
            }

            if (i < 0)
            {
                return (-1, -1);
            }

            return (i + 1, count);
        }

        private static void SwapSpots(FileSystemItem?[] items, int startIdx1, int startIdx2, int length)
        {
            for (int i = 0; i < length; i++)
            {
                int idx1 = startIdx1 + i;
                int idx2 = startIdx2 + i;

                FileSystemItem? tmp = items[idx1];
                items[idx1] = items[idx2];
                items[idx2] = tmp;
            }
        }

        private List<FileSystemItem?> CompactFileSystem()
        {
            FileSystemItem?[] items = Items.ToArray();

            int i = items.Length - 1;
            while (true)
            {
                if (i < 0)
                {
                    break;
                }

                (int idx, int count) file = FindFileFromEnd(items, i);

                if (file.idx < 0)
                {
                    break;
                }

                int emptySpotsIdx = FindMatchingEmptySpace(items, file.count, file.idx);

                if (emptySpotsIdx < 0)
                {
                    i = file.idx - 1;
                    continue;
                }

                SwapSpots(items, file.idx, emptySpotsIdx, file.count);
                i = file.idx - 1;
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

                if (item != null)
                {
                    checksum += i * item.ID;
                }
            }

            return checksum;
        }
    }

    class FileSystemItem(int id)
    {
        public int ID = id;
    }
}
