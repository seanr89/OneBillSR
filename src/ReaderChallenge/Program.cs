// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

var fileLength = File.ReadAllLines("../DataSet/weather-data.csv").Length;

Console.WriteLine($"Done for {fileLength:N0} filesize");

var availableThreads = Environment.ProcessorCount * 2;
Console.WriteLine($"Available threads: {availableThreads}");