using System.Diagnostics;
using advent_of_code_day16;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
checked // Let's check for overflows
{
    await new Day16().Execute();
}
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
