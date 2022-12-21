using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace advent_of_code_day16;

public class Day16
{
    Regex r;

    public class Valve : INode
    {
        public static float xCounter = 0;
        public Valve()
        {
        }

        public Valve(string line, Regex r)
        {
            var result = r.Match(line);              
            Id = result.Groups["v"].Value;
            var bytes = new byte[]{ (byte)Id[0], (byte)Id[1] };
            ShortId = Encoding.Unicode.GetString(bytes)[0];
            Rate = int.Parse(result.Groups["fr"].Value);
            OtherValveIds = result.Groups["ov"].Captures.Select(c => c.Value).ToHashSet();
        }
        public char ShortId { get; set; }
        public string Id { get; set; }
        public int Rate { get; set; }
        public HashSet<string> OtherValveIds {get; set;} = new HashSet<string>();
        public HashSet<Valve> OtherValves { get; set; } = new HashSet<Valve>();

        public HashSet<T> GetAdjacenctNodes<T>() where T : INode
        {
            return OtherValves.Cast<T>().ToHashSet();
        }

        public void Resolve(List<Valve> valves)
        {
            foreach (var id in OtherValveIds)
            {
                var valve = valves.Single(v => v.Id == id);
                OtherValves.Add(valve);
            }
        }
    }

    public Day16()
    {
        string pat = @"Valve (?<v>\w\w) has flow rate=(?<fr>\d*); tunnels? leads? to valves? ((?<ov>\w\w)(, )?)+";
        r = new Regex(pat, RegexOptions.Compiled);
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day16.txt");
        var valves = lines.Select(l => new Valve(l, r)).ToList();
        foreach (var valve in valves)
            valve.Resolve(valves);

        var openValves = valves
            .Where(v => v.Rate > 0)
            .OrderByDescending(v => v.Rate)
            .ToList();
                
        var aa = valves.First(v => v.Id == "AA");
        var result = Tick(aa, 30, 0, 0, openValves);
        System.Console.WriteLine($"part1: {result}");
        //1991 is not the right answer
        //That's not the right answer. Curiously, it's the right answer for someone else; you might be logged in to the wrong account or just unlucky.
    }

    public interface INode {
        public HashSet<T> GetAdjacenctNodes<T>() where T : INode;
    }

    // Thanks koder dojo
    // https://www.koderdojo.com/blog/breadth-first-search-and-shortest-path-in-csharp-and-net-core
    public Func<T, IEnumerable<T>> BreadthFirstSearch<T>(T start) where T : class, INode {
        var previous = new Dictionary<T, T>();
        var queue = new Queue<T>();
        queue.Enqueue(start);
        while (queue.Count > 0) {
            var vertex = queue.Dequeue();            
            foreach(var neighbor in vertex.GetAdjacenctNodes<T>())
            {
                if (previous.ContainsKey(neighbor))
                    continue;
    
                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        // So you do that once and then are able to do it again?
        Func<T, IEnumerable<T>> shortestPath = (v) => {
            var path = new List<T>{};

            var current = v;
            while (!current.Equals(start)) {
                path.Add(current);
                current = previous[current];
            };

            path.Add(start);
            path.Reverse();
            return path;
        };

        return shortestPath;
    }

    Dictionary<Valve,
        Func<Valve, IEnumerable<Valve>>> traversals = new Dictionary<Valve, Func<Valve, IEnumerable<Valve>>>();
    public Dictionary<string, int> searchCache = new Dictionary<string, int>();
    public int FindPath(Valve start, Valve end)
    {
        var cacheString = $"{start.Id}{end.Id}";  // Paths in both directions are the same!
        if (end.ShortId < start.ShortId)
        {
            cacheString = $"{end.Id}{start.Id}";
        }
        
        int result = 0;
        if (!searchCache.TryGetValue(cacheString, out result))
        {
            Func<Valve, IEnumerable<Valve>> lookupFunction;
            if (!traversals.TryGetValue(start, out lookupFunction))
            {
                lookupFunction = BreadthFirstSearch<Valve>(start);
                traversals.Add(start, lookupFunction);
            }

            var path = lookupFunction(end);
            result = path.Count() - 1;
            searchCache.Add(cacheString, result);
        }

        return result;
    }

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
                foreach (var item in openValves)
                {
                    if (item == valve)
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
                    else
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

                if (options.Count > 0)
                {
                    var best = options.OrderByDescending(t=>t.TotalValue).ToList();
                    foreach (var item in best.Take(8))
                    {
                        result = int.Max(result, item.Func());
                    }
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
