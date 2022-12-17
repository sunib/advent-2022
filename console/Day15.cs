using System.Numerics;
using System.Text.RegularExpressions;
namespace advent_of_code_day15;

public class Day15
{
    public class Sensor
    { 
        public Sensor(string line, Regex r)
        {
            var match = r.Match(line);
            Location = new Point(match.Groups["sx"].Value, match.Groups["sy"].Value);
            ClosestBeacon = new Point(match.Groups["bx"].Value, match.Groups["by"].Value);
            Distance = CalculateManhattanDistance(ClosestBeacon);
        }

        // We have a whole mahattan field that is empty, so we should use that as a big or to check it!
        public Point Location { get; set; }
        public Point ClosestBeacon { get; set; }
        public int Distance { get; set; }

        // The sum of abs x and y is our distance, so we can deduce if it's in range and if we have two or one matches.
        public Tuple<int,int> IntersectWith(int y)
        {
            // How much do we have left?
            var absY = int.Abs(this.Location.Y - y);
            if (absY == Distance)   // One point
            {
                return new Tuple<int, int>(this.Location.X, this.Location.X);
            }
            else if  (absY < Distance) {    // Two points
                var x = Distance - absY;
                return new Tuple<int, int>(this.Location.X - x, this.Location.X + x);
            }
            
            return null;    // No intersect
        }

        public int CalculateManhattanDistance(Point point)
        {
            return int.Abs(point.X - Location.X) + int.Abs(point.Y - Location.Y);
        }
    }

    public class Point
    {
        public Point(string x, string y)
        {
            this.X = int.Parse(x);
            this.Y = int.Parse(y);
        }
        
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    Regex r;

    public Day15()
    {
        string pat = @"Sensor at x=(?<sx>-?\d*), y=(?<sy>-?\d*): closest beacon is at x=(?<bx>-?\d*), y=(?<by>-?\d*)";
        r = new Regex(pat, RegexOptions.Compiled);
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day15.txt");
        var sensors = lines.Select(l => new Sensor(l, r)).ToList();

        var tuningFreq = Part2(sensors);
        System.Console.WriteLine($"part2: {tuningFreq}");
        // Not right:
        // 320708895
        // 13267474686239 -> thank you biginteger!
    }

    // Part 2
    // Distress beacon is not a closest beacon for any sensor!
    // x,y is not bigger then 4000000
    // answer = tuning frequency: x*4000000+y
    // search space is smallar x and y at most 20
    // beacon can only ba x=14, y=11
    private static BigInteger Part2(List<Sensor> sensors)
    {
        var tuningFreq = 0;
        for (int y = 0; y <= 4000000; y++)
        {
            if (y % 10000 == 0)
                System.Console.WriteLine($"y: {y}");

            var intersections = sensors
                .Select(s => s.IntersectWith(y))
                .Where(t => t != null)
                .OrderBy(s => s.Item1)
                .ToList();

            for (int x = 0; x <= 4000000; x++)
            {
                var current = intersections.FirstOrDefault(i => x >= i.Item1 && x <= i.Item2);
                if (current != null)
                {
                    x = current.Item2;
                    //intersections.Remove(current);
                }
                else 
                {
                    System.Console.WriteLine("Bam");
                    var result = new BigInteger(4000000);
                    return result * new BigInteger(x) + new BigInteger(y);
                }
            }
        }

        return tuningFreq;
    }
}
