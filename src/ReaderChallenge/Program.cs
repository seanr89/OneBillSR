// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.Diagnostics;

string fileName = "../DataSet/weather-data.csv";

var fileLength = File.ReadAllLines(fileName).Length;

Console.WriteLine($"Done for {fileLength:N0} filesize");

var availableThreads = Environment.ProcessorCount * 2;
Console.WriteLine($"Available threads: {availableThreads}");

Stopwatch sw = new Stopwatch();
sw.Start();

// string[] FileContents = File.ReadAllLines(fileName);
int count = 0;

Parallel.ForEach(File.ReadLines(fileName), (line, _, lineNumber) =>
{
    // your code here
    count++;
});

Console.WriteLine($"Read file in {sw.ElapsedMilliseconds}ms");
sw.Stop();