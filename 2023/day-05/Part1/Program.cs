using Sprache;

namespace Part1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../../input.txt";
            string input = File.ReadAllText(path);

            SeedInformation seedInformation = InputParser.ParseInput(input);
            Func<long, long> seedToSoilMapping = MappingToDictionary(seedInformation.SeedToSoilMap);
            Func<long, long> soilToFertilizerMapping = MappingToDictionary(seedInformation.SoilToFertilizerMap);
            Func<long, long> fertilizerToWaterMapping = MappingToDictionary(seedInformation.FertilizerToWaterMap);
            Func<long, long> waterToLightMapping = MappingToDictionary(seedInformation.WaterToLightMap);
            Func<long, long> lightToTemperatureMapping = MappingToDictionary(seedInformation.LightToTemperatureMap);
            Func<long, long> temperatureToHumidityMapping = MappingToDictionary(seedInformation.TemperatureToHumidityMap);
            Func<long, long> humidityToLocationMapping = MappingToDictionary(seedInformation.HumidityToLocationMap);

            long result = seedInformation.Seeds.Select(x => seedToSoilMapping(x))
                                               .Select(x => soilToFertilizerMapping(x))
                                               .Select(x => fertilizerToWaterMapping(x))
                                               .Select(x => waterToLightMapping(x))
                                               .Select(x => lightToTemperatureMapping(x))
                                               .Select(x => temperatureToHumidityMapping(x))
                                               .Select(x => humidityToLocationMapping(x))
                                               .Order()
                                               .First();
        }

        private static Func<long, long> MappingToDictionary(List<(long, long, long)> mappings)
        {
            return num => {
                foreach (var mapping in mappings)
                {
                    long destinationRangeStart = mapping.Item1;
                    long sourceRangeStart = mapping.Item2;
                    long rangeLength = mapping.Item3;

                    if (num >= sourceRangeStart && num < sourceRangeStart + rangeLength)
                    {
                        long offset = num - sourceRangeStart;
                        return destinationRangeStart + offset;
                    }
                }

                return num;
            };
        }
    }

    public class SeedInformation
    {
        public List<long> Seeds;
        public List<(long, long, long)> SeedToSoilMap;
        public List<(long, long, long)> SoilToFertilizerMap;
        public List<(long, long, long)> FertilizerToWaterMap;
        public List<(long, long, long)> WaterToLightMap;
        public List<(long, long, long)> LightToTemperatureMap;
        public List<(long, long, long)> TemperatureToHumidityMap;
        public List<(long, long, long)> HumidityToLocationMap;

        public SeedInformation(IEnumerable<long> seeds, IEnumerable<(long, long, long)> seedToSoilMap, IEnumerable<(long, long, long)> soilToFertilizerMap, IEnumerable<(long, long, long)> fertilizerToWaterMap, IEnumerable<(long, long, long)> waterToLightMap, IEnumerable<(long, long, long)> lightToTemperatureMap, IEnumerable<(long, long, long)> temperatureToHumidityMap, IEnumerable<(long, long, long)> humidityToLocationMap)
        {
            Seeds = seeds.ToList();
            SeedToSoilMap = seedToSoilMap.ToList();
            SoilToFertilizerMap = soilToFertilizerMap.ToList();
            FertilizerToWaterMap = fertilizerToWaterMap.ToList();
            WaterToLightMap = waterToLightMap.ToList();
            LightToTemperatureMap = lightToTemperatureMap.ToList();
            TemperatureToHumidityMap = temperatureToHumidityMap.ToList();
            HumidityToLocationMap = humidityToLocationMap.ToList();
        }
    }

    public static class InputParser
    {
        public static SeedInformation ParseInput(string input)
        {
            return seedInformationParser.Parse(input);
        }

        private static readonly Parser<(long, long, long)> longTripleParser =
            from first in Parse.Digit.AtLeastOnce().Text()
            from space1 in Parse.Char(' ')
            from second in Parse.Digit.AtLeastOnce().Text()
            from space2 in Parse.Char(' ')
            from third in Parse.Digit.AtLeastOnce().Text()
            select (long.Parse(first), long.Parse(second), long.Parse(third));

        private static readonly Parser<IEnumerable<(long, long, long)>> mappingsParser =
            longTripleParser.DelimitedBy(Parse.Char('\n'));

        private static readonly Parser<SeedInformation> seedInformationParser =
            from seedTitle in Parse.String("seeds: ")
            from seedNumbers in Parse.Digit.AtLeastOnce().Text().DelimitedBy(Parse.Char(' '))
            from newlines in Parse.String("\n\n")
            from seedToSoilTitle in Parse.String("seed-to-soil map:\n")
            from seedToSoilMappings in mappingsParser
            from newlines2 in Parse.String("\n\n")
            from soilToFertilizerTitle in Parse.String("soil-to-fertilizer map:\n")
            from soilToFertilizerMappings in mappingsParser
            from newlines3 in Parse.String("\n\n")
            from fertilizerToWaterTitle in Parse.String("fertilizer-to-water map:\n")
            from fertilizerToWaterMappings in mappingsParser
            from newlines4 in Parse.String("\n\n")
            from waterToLightTitle in Parse.String("water-to-light map:\n")
            from waterToLightMappings in mappingsParser
            from newlines5 in Parse.String("\n\n")
            from lightToTemperatureTitle in Parse.String("light-to-temperature map:\n")
            from lightToTemperatureMappings in mappingsParser
            from newlines6 in Parse.String("\n\n")
            from temperatureToHumidityTitle in Parse.String("temperature-to-humidity map:\n")
            from temperatureToHumidityMappings in mappingsParser
            from newlines7 in Parse.String("\n\n")
            from humidityToLocationTitle in Parse.String("humidity-to-location map:\n")
            from humidityToLocationMappings in mappingsParser
            from newlines8 in Parse.String("\n").End()
            select new SeedInformation(seedNumbers.Select(long.Parse), seedToSoilMappings, soilToFertilizerMappings, fertilizerToWaterMappings, waterToLightMappings, lightToTemperatureMappings, temperatureToHumidityMappings, humidityToLocationMappings);

    }
}
