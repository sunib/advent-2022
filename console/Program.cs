using System.Diagnostics;
using advent_of_code_day14;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
await new Day14().Execute();
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
