using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Helpers.BreadthFirstSearch;

namespace advent_of_code_day16;

public class Day16
{
    Regex r;
    Valve[] openValves;
    BreadthFirstSearch<Valve> bfs = new BreadthFirstSearch<Valve>();

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

        public void Resolve(List<Valve> valves, List<string> output)
        {
            foreach (var id in OtherValveIds)
            {
                var valve = valves.Single(v => v.Id == id);
                OtherValves.Add(valve);
                output.Add($"{this.Id}-{id}");
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
        
        var output = new List<string>();
        foreach (var valve in valves)
            valve.Resolve(valves, output);
        System.IO.File.WriteAllLines("test-output.txt", output);

        openValves = valves     // We can use this to lookup as well!
            .Where(v => v.Rate > 0)
            .OrderByDescending(v => v.Rate)
            .ToArray();

        // We limit it no two 32 and give them a bit id
        BitVector32 openValvesMask = new BitVector32();
        for (int i = 0; i < openValves.Length; i++)
        {
            openValvesMask[1 << i] = true;
        }        
                        
        var aa = valves.First(v => v.Id == "AA");
        var result = Tick(aa, 30, openValvesMask);
        System.Console.WriteLine($"part1: {result}");
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

    public IEnumerable<Valve> GetOpenValves(BitVector32 openValvesMask)
    {
        for (int i = 0; i < openValves.Length; i++)
            if (openValvesMask[1<<i])
                yield return openValves[i];
    }

    public BitVector32 CloseValve(Valve valve, BitVector32 openValvesMask)
    {
        var result = new BitVector32(openValvesMask);
        for (int i = 0; i < openValves.Length; i++)
            if (openValves[i] == valve)
            {
                result[1 << i] = false;
                return result;
            }
        
        throw new InvalidDataException("This valve was already closed");
    }

    public record CachedTick (Valve Valve, int MinutesLeft, BitVector32 openValvesMask);
    private Dictionary<CachedTick, int> tickCache = new Dictionary<CachedTick, int>();    

    public int highestResult = 0;
    public int Tick(Valve valve, int minutesLeft, BitVector32 openValvesMask)
    {
        var result = 0;
        var parameters = new CachedTick(valve, minutesLeft, openValvesMask);
        if (tickCache.TryGetValue(parameters, out result) || 
            openValvesMask.Data == 0 || 
            minutesLeft <= 0)
        {
            return result;
        }

        // First calculate some routes and multiply it. In the calculation it does make sense to take into account our current location (opening). It could be more efficeient to visit a nearby big valve first.
        var results = new List<int>();
        foreach (var item in GetOpenValves(openValvesMask))
        {
            if (item == valve)
            {   
                // We should also decide not do it!
                results.Add(
                    valve.Rate * (minutesLeft - 1) + 
                    Tick(valve, minutesLeft - 1, CloseValve(valve, openValvesMask)));
            }
            else
            {
                var minutes = bfs.FindPath(valve, item);
                if (minutesLeft > minutes)
                {
                    results.Add(Tick(item, minutesLeft - minutes, openValvesMask));
                }
            }
        }

        if (results.Count > 0)
            result += results.Max();

        if (result > highestResult)
        {
            highestResult = result;
            System.Console.WriteLine("highest result: " + highestResult);
        }

        tickCache.Add(parameters, result);
        return result;
    }
}
