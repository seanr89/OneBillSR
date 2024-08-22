// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
Console.WriteLine("Reader Started");

string fileName = "../DataSet/weather-data.csv";

var fileLength = File.ReadAllLines(fileName).Length;

Console.WriteLine($"Done for {fileLength:N0} filesize");

var availableThreads = Environment.ProcessorCount * 2;
//var availableThreads = Environment.ProcessorCount;
Console.WriteLine($"Available threads: {availableThreads}");

var stationData = File.ReadAllLines("../DataSet/weather-stations.csv")
    .Select(x => x.Split(";"))
    .Select(x => new Station(x[0]))
    .ToList();

Stopwatch sw = new Stopwatch();
sw.Start();

// string[] FileContents = File.ReadAllLines(fileName);
int count = 0;

HashSet<StationStats> stats = new();
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
    stats.Add(stationStat);
}

Parallel.ForEach(File.ReadLines(fileName), (line, _, lineNumber) =>
{
    // your code here
    count++;
    if(count % 50000 == 0)
    {
        Console.WriteLine($"Processed {count} records");
    }
    var dataSplit = line.Split(";");
    DataRecord rec = new(dataSplit[0], (int) double.Parse(dataSplit[1]) * 100);
    var stat = stats.First(x => x.Name == rec.name);
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