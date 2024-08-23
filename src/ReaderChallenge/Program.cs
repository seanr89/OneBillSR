// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using System.Diagnostics;
Console.WriteLine("Reader Started");

string fileName = "../DataSet/weather-data.csv";

// var fileLength = File.ReadAllLines(fileName).Length;

// Console.WriteLine($"Done for {fileLength:N0} filesize");

var availableThreads = Environment.ProcessorCount * 2;
//var availableThreads = Environment.ProcessorCount;
//Console.WriteLine($"Available threads: {availableThreads}");

var stationData = File.ReadAllLines("../DataSet/weather-stations.csv")
    .Select(x => x.Split(";"))
    .Select(x => new Station(x[0]))
    .ToList();

stationData = stationData.Distinct().ToList();

Stopwatch sw = new Stopwatch();
sw.Start();

int count = 0;

Dictionary<string, StationStats> stats = new();
for(int i = 0; i < stationData.Count; i++)
{
    var station = stationData[i];
    var stationStat = new StationStats
    {
        Name = station.name,
        Count = 0,
        Lowest = 0,
        Highest = 0,
        Average = 0,
        Total = 0
    };
    stats.Add(station.name, stationStat);
}
// Console.WriteLine($"Created {stats.Count} stats");

Parallel.ForEach(File.ReadLines(fileName), (line, _, lineNumber) =>
{
    count++;
    // if(count % 50000 == 0)
    // {
    //     Console.WriteLine($"Processed {count} records");
    // }
    var dataSplit = line.Split(";");
    DataRecord rec = new(dataSplit[0], double.Parse(dataSplit[1]));
    var stat = stats[rec.name];
    if(stat == null)
        return;

    stat.Count++;
    stat.Total += rec.value;
    if (stat.Lowest == 0 || rec.value < stat.Lowest)
    {
        stat.Lowest = rec.value;
    }
    if (rec.value > stat.Highest)
    {
        stat.Highest = rec.value;
    }
    stat.Average = (double)stat.Total / stat.Count;
});

Console.WriteLine($"Processed {count} records");
Console.WriteLine($"Read file in {sw.ElapsedMilliseconds}ms");
sw.Stop();

var orderedStats = stats.Values.OrderBy(x => x.Total).ToList();
orderedStats = orderedStats.Where(x => x.Count > 0).ToList();

// Pull and process the top 50 records
orderedStats = orderedStats.Take(50).ToList();
foreach(var stat in orderedStats)
{
    Console.WriteLine($"{stat.Name} - Count: {stat.Count}, Lowest: {stat.Lowest}, Highest: {stat.Highest}, Average: {stat.Average:N2}");
}