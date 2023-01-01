using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Helpers.BreadthFirstSearch;

namespace advent_of_code_day16;

public class Valve : INode
{
    public static float xCounter = 0;
    public Valve(string line, Regex r)
    {
        var result = r.Match(line);              
        Id = result.Groups["v"].Value;
        Rate = int.Parse(result.Groups["fr"].Value);
        OtherValveIds = result.Groups["ov"].Captures.Select(c => c.Value).ToHashSet();
    }

    public string Id { get; set; }
    public int? OpenIndex { get; set;}   // Where is this located in the openValves?
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

public class Day16
{
    Regex r;
    Valve[] openValves;
    BreadthFirstSearch<Valve> bfs = new BreadthFirstSearch<Valve>();

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
            openValves[i].OpenIndex = i;
        }        
                        
        var aa = valves.First(v => v.Id == "AA");
        var result = Tick(new Opener(aa, 0), new Opener(aa, 0), 26, openValvesMask);
        System.Console.WriteLine($"part2: {result}");
    }
    
    public IEnumerable<Valve> GetOpenValves(BitVector32 openValvesMask)
    {
        for (int i = 0; i < openValves.Length; i++)
            if (openValvesMask[1<<i])
                yield return openValves[i];        
    }

    public bool TryCloseValve(Valve valve, ref BitVector32 openValvesMask)
    {
        for (int i = 0; i < openValves.Length; i++)
            if (openValves[i] == valve && openValvesMask[1<<i])     // Throw error if you try it for a second time?
            {
                openValvesMask[1 << i] = false;
                return true;
            }
        
        return false;
    }

    public int CloseValve(Opener current, int minutesLeft, ref BitVector32 openValvesMask)
    {
        if (TryCloseValve(current.dest, ref openValvesMask))
        {
            return minutesLeft * current.dest.Rate;
        }

        return 0;
    }

    public record Opener(Valve dest, int minutesLeft, bool isOpening = false);
    public record CachedTick (Opener A, Opener B, int minutesLeft, BitVector32 openValvesMask);
    private Dictionary<CachedTick, int> tickCache = new Dictionary<CachedTick, int>();    
    public int highestResult = 0;
    public int Tick(Opener A, Opener B, int minutesLeft, BitVector32 openValvesMask)
    {
        var result = 0;
        var parameters = new CachedTick(A, B, minutesLeft, openValvesMask);
        if (tickCache.TryGetValue(parameters, out result) || 
            openValvesMask.Data == 0 || 
            minutesLeft <= 0)
        {
            // Take the smallest minutes left from the two: also assume that the smallest of the two is the active one. If they are equeal we will have to run the code twice.
            return result;
        }

        // If one of them is opening then we first close it and take our credits!
        if (A.isOpening)
            result += CloseValve(A, minutesLeft, ref openValvesMask);

        if (B.isOpening)
            result += CloseValve(B, minutesLeft, ref openValvesMask);

        int minutesSincePrevious = int.Min(A.minutesLeft, B.minutesLeft);
        Opener overrideA = null;
        Opener overrideB = null;
        if (A.minutesLeft < B.minutesLeft)
        {
            // A arrived
            overrideB = new Opener(B.dest, B.minutesLeft - minutesSincePrevious);   // B is still moving 
        }
        else if (B.minutesLeft < A.minutesLeft)
        {
            overrideA = new Opener(A.dest, A.minutesLeft - minutesSincePrevious);   // A is still moving 
        }

        var valvesA = new BitVector32(openValvesMask);
        var valvesB = new BitVector32(openValvesMask);
        
        if (overrideA != null)
        {
            valvesA = new BitVector32();
            valvesA[1 << overrideA.dest.OpenIndex.Value] = true;    // Override to only visit the overriden value
            valvesB[1 << overrideA.dest.OpenIndex.Value] = false;   // Do not try to visit the one that is already getting a visit
        }

        if (overrideB != null)
        { 
            valvesB = new BitVector32();
            valvesB[1 << overrideB.dest.OpenIndex.Value] = true; 
            valvesA[1 << overrideB.dest.OpenIndex.Value] = false; 
        }

        var results = new List<int>();
        foreach (var decisionA in GetOpenValves(valvesA).Select(p => DecideStep(A, p, overrideA)).OrderByDescending(d => (minutesLeft - d.minutesLeft) * d.dest.Rate).Take(4))       // Moving to yourself because it's an open valve is off course also a valid move that we can include!
        {
            var valvesBtemp = new BitVector32(valvesB);
            valvesBtemp[1 << decisionA.dest.OpenIndex.Value] = false;   // Don't visit what we do in the outer loop already!
            foreach (var decisionB in GetOpenValves(valvesBtemp).Select(p => DecideStep(B, p, overrideB)).OrderByDescending(d => (minutesLeft - d.minutesLeft) * d.dest.Rate).Take(4))
            {
                results.Add(
                    Tick(
                        decisionA,
                        decisionB, 
                        minutesLeft - int.Min(decisionA.minutesLeft, decisionB.minutesLeft), 
                        openValvesMask));
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

    private Opener DecideStep(Opener current, Valve proposed, Opener proposedOverride)
    {
        if (proposedOverride != null)
        {
            return proposedOverride;
        }
        else
        {
            if (current.dest == proposed)
            {
                return new Opener(proposed, 1, isOpening: true);
            }
            else 
            {
                var distance = bfs.FindPath(current.dest, proposed);
                return new Opener(proposed, distance);
            }
        }
    }
}

