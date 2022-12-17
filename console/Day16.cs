using System.Numerics;
using System.Text.RegularExpressions;
namespace advent_of_code_day16;

public class Day16
{
    Regex r;

    public class Valve
    {
        public Valve()
        {
        }

        public Valve(string line, Regex r)
        {
            var result = r.Match(line);              
            Id = result.Groups["v"].Value;
            Rate = int.Parse(result.Groups["fr"].Value);
            OtherValveIds = result.Groups["ov"].Captures.Select(c => c.Value).ToList();
        }

        public string Id { get; set; }
        public int Rate { get; set; }
        public List<string> OtherValveIds {get; set;} = new List<string>();
        public List<Valve> OtherValves { get; set; } = new List<Valve>();

        public void Resolve(List<Valve> valves)
        {
            foreach (var id in OtherValveIds)
                OtherValves.Add(valves.Single(v => v.Id == id));
        }

    }

    private int highestReleasing = 0;
    private int highestResult = 0;

    public Day16()
    {
        string pat = @"Valve (?<v>\w\w) has flow rate=(?<fr>\d*); tunnels? leads? to valves? ((?<ov>\w\w)(, )?)+";
        r = new Regex(pat, RegexOptions.Compiled);
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day16-simple.txt");
        var valves = lines.Select(l => new Valve(l, r)).ToList();
        foreach (var valve in valves)
            valve.Resolve(valves);

        var openValves = valves
            .Where(v => v.Rate > 0)
            .OrderByDescending(v => v.Rate)
            .ToList();

//1235
// Guess: 2704 -> that is to high?!
        
        var aa = valves.First(v => v.Id == "AA");
        var result = Tick(aa, 30, 0, 0, openValves);        
        System.Console.WriteLine($"part1: {result}");
        
    }

    public int Tick(Valve valve, int minutesLeft, int releasing, int total, List<Valve> openValves)
    {
        minutesLeft--;
        if (minutesLeft <= 0)
            return total;
        
        total = total + releasing;
        var result = total;
        if (openValves.Count > 0)
        {
            if (openValves.Contains(valve))
            {
                var openValvesMinus1 = openValves.ToList();
                openValvesMinus1.Remove(valve);
                result = int.Max(result,
                    Tick(valve, minutesLeft, releasing + valve.Rate, total, openValvesMinus1));
            }
            else    // This else might be to much, it could be more effecient to walk by a valve without opening it if the score is high enough on the other one
            {
                result = int.Max(result,
                    valve.OtherValves.Max(v => Tick(v, minutesLeft, releasing, total, openValves)));
            }
        }
        else 
        {
            // Nothing happens, wait until it's done
            // We should also be able to calculate when it's done and increase the stuff a bit.
            return result + releasing * minutesLeft;
        }

        return result;
    }

}
