using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using EMK.Cartography;

namespace advent_of_code_day16;

public class Day16
{
    Regex r;
    

    public class Valve
    {
        public static float xCounter = 0;
        public Valve()
        {
        }

        public Valve(string line, Regex r, Graph g)
        {
            var result = r.Match(line);              
            Id = result.Groups["v"].Value;
            var bytes = new byte[]{ (byte)Id[0], (byte)Id[1] };
            ShortId = Encoding.Unicode.GetString(bytes)[0];
            Rate = int.Parse(result.Groups["fr"].Value);
            OtherValveIds = result.Groups["ov"].Captures.Select(c => c.Value).ToList();
            Node = g.AddNode(ShortId,0,0);
        }

        public Node Node {get;set;}
        public char ShortId { get; set; }
        public string Id { get; set; }
        public int Rate { get; set; }
        public List<string> OtherValveIds {get; set;} = new List<string>();
        public List<Valve> OtherValves { get; set; } = new List<Valve>();

        public void Resolve(List<Valve> valves, Graph g)
        {
            foreach (var id in OtherValveIds)
            {
                var valve = valves.Single(v => v.Id == id);
                OtherValves.Add(valve);
                g.Add2Arcs(valve.Node, Node, 1);
            }
        }
    }

    public Day16()
    {
        string pat = @"Valve (?<v>\w\w) has flow rate=(?<fr>\d*); tunnels? leads? to valves? ((?<ov>\w\w)(, )?)+";
        r = new Regex(pat, RegexOptions.Compiled);
    }

    public Graph g = new Graph();

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day16-simple.txt");
        var valves = lines.Select(l => new Valve(l, r, g)).ToList();
        foreach (var valve in valves)
            valve.Resolve(valves, g);

        var openValves = valves
            .Where(v => v.Rate > 0)
            .OrderByDescending(v => v.Rate)
            .ToList();
                
        var aa = valves.First(v => v.Id == "AA");
        //1235
        // Guess: 2704 -> that is to high?!
        var result = Tick(aa, 32, 0, 0, openValves);
        System.Console.WriteLine($"part1: {result}");
    }

    public Dictionary<string, int> searchCache = new Dictionary<string, int>();
    // So we will only use this to find the shortest route but still be a brutforcing it.
    public int FindPath(Valve start, Valve end)
    {
        var cacheString = $"{start.ShortId}{end.ShortId}";  // Paths in both directions are the same!
        if (end.ShortId < start.ShortId)
        {
            cacheString = $"{end.ShortId}{start.ShortId}";
        }
        
        int result = 0;
        if (!searchCache.TryGetValue(cacheString, out result))
        {
            AStar AS = new AStar(g);
            AS.SearchPath(start.Node, end.Node);
            searchCache.Add(cacheString, AS.PathByNodes.Count());
            return AS.PathByNodes.Count();  // During the traversal we should actually also decide if we want to open valves! Might be worth it. Or not?
        }

        return result;
    }

    // We could also save the previous calculations with the same input data. The only thing is that we should not keep minutesleft, total and releasing off course.

    public class DecisionOption
    {
        public DecisionOption(int steps, int totalValue, Valve next, Func<int> func)
        {
            TotalValue = totalValue;
            Steps = steps;
            Next = next;
            Func = func;
        }
        public Func<int> Func;
        public int Steps;
        public int TotalValue { get; set; }
        public Valve Next { get; set; }
    }

    public int highestResult = 0;
    public int Tick(Valve valve, int minutesLeft, int releasing, int total, List<Valve> openValves)
    {
        var result = total;
        if (minutesLeft > 0)
        {
            if (openValves.Count > 0)
            {
                // First calculate some routes and multiply it. In the calculation it does make sense to take into account our current location (opening). It could be more efficeient to visit a nearby big valve first.
                var options = new List<DecisionOption>();
                if (openValves.Contains(valve))
                {
                    options.Add(new DecisionOption(1, valve.Rate*minutesLeft, valve, () => {
                        var openValvesMin1 = openValves.ToList();
                        openValvesMin1.Remove(valve);
                        return Tick(valve, 
                                minutesLeft - 1, 
                                releasing + valve.Rate, 
                                total + releasing, 
                                openValvesMin1);
                    }));                    
                }
                
                foreach (var item in openValves)
                {
                    if (item != valve)  // Don't investigate yourself: will never finish!
                    {
                        var minutes = FindPath(valve, item);
                        if (minutesLeft > minutes)
                        {
                            options.Add(new DecisionOption(minutes, item.Rate*(minutesLeft - minutes), item, () => {
                            return Tick(item, 
                                minutesLeft - minutes, 
                                releasing,
                                total + (releasing * minutes), 
                                openValves);
                        }));
                        }
                    }
                }

                var best = options.OrderByDescending(t=>t.TotalValue).ToList();
                foreach (var step in best.Take(6))
                {
                    result = int.Max(result, step.Func());
                }                
            }
        }
        
        // Let's check if something has happened, if not than then we can skip out 
        if (result <= total) {
            // Nothing happens, wait until it's done
            // We should also be able to calculate when it's done and increase the stuff a bit.
            result += releasing * minutesLeft;
            minutesLeft = 0;
        }

        if (result > highestResult)
        {
            highestResult = result;
            System.Console.WriteLine("highest result: " + highestResult);
        }

        return result;
    }

}
