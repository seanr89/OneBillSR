using System.Diagnostics;
Console.WriteLine("Reader Started");

string fileName = "../DataSet/weather-data.csv";

var availableThreads = Environment.ProcessorCount * 2;

List<Station> stationData = InitialiseStations();

stationData = stationData.Distinct().ToList();
int count = 0;

Stopwatch sw = new Stopwatch();
sw.Start();


// Initialize the stats dictionary and build the initial stats records
Dictionary<string, StationStats> stats = InitializeStats(stationData);

Parallel.ForEach(File.ReadLines(fileName), (line, _, lineNumber) =>
{
    // increment the count
    count++;
    // Split and build the data record object
    var dataSplit = line.Split(";");
    DataRecord rec = new(dataSplit[0], double.Parse(dataSplit[1]));
    // Search for and detail the stats record
    var stat = stats[rec.name];
    if(stat == null)
        return;

    // Update the stats record for lowest, highest, average, and total etc...
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
// end of parallel processing

Console.WriteLine($"Processed {count:N0} records");
Console.WriteLine($"Read file in {sw.ElapsedMilliseconds}ms");
sw.Stop();

// now we need to sort the stats and output the top 50 etc...
var orderedStats = stats.Values.OrderBy(x => x.Total).ToList();
orderedStats = orderedStats.Where(x => x.Count > 0).ToList();

// Pull and process the top 50 records
orderedStats = orderedStats.Take(50).ToList();
foreach(var stat in orderedStats)
{
    Console.WriteLine($"{stat.Name} - Count: {stat.Count}, Lowest: {stat.Lowest}, Highest: {stat.Highest}, Average: {stat.Average:N2}");
}

/// <summary>
/// 
/// </summary>
/// <param name="stationData"></param>
/// <returns></returns>
static Dictionary<string, StationStats> InitializeStats(List<Station> stationData)
{
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
    return stats;
}

static List<Station> InitialiseStations()
{
    return File.ReadAllLines("../DataSet/weather-stations.csv")
        .Select(x => x.Split(";"))
        .Select(x => new Station(x[0]))
        .ToList();
}