var stationData = File.ReadAllLines("weather-stations.csv")
    .Select(x => x.Split(";"))
    .Select(x => new Station(x[0]))
    .ToList();

int count = 25000;


for(int x = 0; x < 100; x++)
{
    var rand = new Random();
    List<WeatherRecord> data = [];
    for(int i = 0; i < count; i++)
    {
        var result = rand.NextDouble().ToString("N2");
        var weatheRec = new WeatherRecord(stationData[rand.Next(stationData.Count)].name, result);
        data.Add(weatheRec);
    }
    File.AppendAllLines("../DataSet/weather-data.csv", data.Select(x => $"{x.name};{x.record}"));
    //Console.WriteLine($"Created {count} records at {x} iteration");
}

var fileLength = File.ReadAllLines("../DataSet/weather-data.csv").Length;

Console.WriteLine($"Done for {fileLength:N0} filesize");