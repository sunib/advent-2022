using System.Diagnostics;
using advent_of_code_day21;

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();        
checked // Let's check for overflows
{
    await new Day21().Execute();
}
stopWatch.Stop();
TimeSpan ts = stopWatch.Elapsed;
System.Console.WriteLine("Executed in " + ts.TotalSeconds + " seconds.");
