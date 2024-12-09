using System.Runtime.InteropServices;

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

        private static int CountEmptySpacesAt(FileSystemItem?[] items, int idx)
        {
            if (idx >= items.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(idx));
            }

            int count = 0;
            while (idx < items.Length && items[idx] == null)
            {
                count++;
                idx++;
            }

            return count;
        }

        private static (int idx, int count) FindMatchingFile(FileSystemItem?[] items, int idx, int length)
        {
            int count = 0;
            for (int i = items.Length - 1; i > idx; i--)
            {
                if (items[i] == null)
                {
                    if (count >= length)
                    {
                        return (i + 1, count);
                    }

                    count = 0;
                }

                count++;
            }

            return (-1, -1);
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

            for (int i = 0; i < items.Length; i++)
            {
                int emptyCount = CountEmptySpacesAt(items, i);
                if (emptyCount == 0)
                {
                    continue;
                }

                (int idx, int count) matchingFile = FindMatchingFile(items, i, emptyCount);
                if (matchingFile.idx == -1)
                {
                    continue;
                }

                SwapSpots(items, i, matchingFile.idx, matchingFile.count);
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
