using System.Diagnostics;
using advent_of_code;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
await new Day6().Execute();
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
