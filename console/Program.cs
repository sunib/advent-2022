using System.Diagnostics;
using advent_of_code_day9;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
await new Day9().Execute();
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
