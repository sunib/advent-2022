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

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day16.txt");
        var g = new Graph();
        var valves = lines.Select(l => new Valve(l, r, g)).ToList();
        foreach (var valve in valves)
            valve.Resolve(valves, g);

        var openValves = valves
            .Where(v => v.Rate > 0)
            .OrderByDescending(v => v.Rate)
            .ToArray();
        var openValvesChars = openValves.Select(v => v.ShortId).ToArray();    
        var openValvesString = new String(openValvesChars, 0, openValves.Length);
                
        var aa = valves.First(v => v.Id == "AA");

        // Take the top 5 and pick the closest
        // Repeat this until we have the cheapset route
        // We might want to randomize this a little bit.

        var minutesLeft = 30;
        while (minutesLeft > 0)
        {





        }

        

        // Just create all options that we could search
        // Then go traverse that thing back and forth! Calculate afterwards the costs

        //1235
        // Guess: 2704 -> that is to high?!
        
        var result = 412;
        System.Console.WriteLine($"part1: {result}");
    }

    // So we will only use this to find the shortest route but still be a brutforcing it.
    public int FindPath(Graph g, Valve start, Valve end)
    {
        AStar AS = new AStar(g);
        AS.SearchPath(start.Node, end.Node);
        return AS.PathByNodes.Count();  // During the traversal we should actually also decide if we want to open valves! Might be worth it. Or not?
    }


    // We could also save the previous calculations with the same input data. The only thing is that we should not keep minutesleft, total and releasing off course.

    public int Tick(Valve valve, int minutesLeft, int releasing, int total, string openValves)
    {
        minutesLeft--;
        var result = total;
        if (minutesLeft > 0)
        {
            total = total + releasing;
            result = total;
            if (openValves.Length > 0)
            {
                var valvePosition = openValves.IndexOf(valve.ShortId);
                if (valvePosition > -1) // So we have the valve in our open list
                {
                    result = int.Max(result,
                        Tick(valve, minutesLeft, releasing + valve.Rate, total, openValves.Remove(valvePosition, 1)));
                }

                
                foreach (var item in valve.OtherValves)
                {
                    result = int.Max(result,
                        Tick(item, minutesLeft, releasing, total, openValves));
                }
            }
            else 
            {
                // Nothing happens, wait until it's done
                // We should also be able to calculate when it's done and increase the stuff a bit.
                result = result + releasing * minutesLeft;
            }
        }
        
        if (result > highestResult)
        {
            highestResult = result;
            System.Console.WriteLine("highest result: " + highestResult);
        }

        return result;
    }

}
