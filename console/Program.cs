using System.Diagnostics;
using advent_of_code_day13;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
await new Day13().Execute();
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
