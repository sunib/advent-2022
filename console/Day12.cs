namespace advent_of_code_day12;
// Following dijkstra
// https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
public class Explorer : ICloneable
{
    public Explorer(MapPoint current, int maxX, int maxY)
    {
        this.Current = current;
        MaxX = maxX;
        MaxY = maxY;
    }

    public MapPoint Current { get; set; }

    public object Clone()
    {
        return new Explorer(Current, MaxX, MaxY);
    }

    public int MaxX { get; set; }
    public int MaxY { get; set; }

    public bool CanVisit(MapPoint current, MapPoint destination, HashSet<MapPoint> unvisited)
    {
        var diff = destination.Height - current.Height;
        if (diff <= 1 && unvisited.Contains(destination) && !destination.IsVisited)  //&& diff >= -1  !destination.IsVisited
        {    
            var newValue = this.Current.TentativeDistanceValue + 1; // All our paths are always 1
            if (newValue < destination.TentativeDistanceValue)
            {
                destination.TentativeDistanceValue = newValue;
            }
            
            return true;
        }
        
        return false;
    }

    public void Visit(HashSet<MapPoint> unvisited, MapPoint[,] map)
    {
        MapPoint next;
        if (TryDown(map, out next)) 
        {
            CanVisit(this.Current, next, unvisited);
        }

        if (TryUp(map, out next)) 
        {
            CanVisit(this.Current, next, unvisited);
        }

        if (TryLeft(map, out next)) 
        {
            CanVisit(this.Current, next, unvisited);
        }

        if (TryRight(map, out next)) 
        {
            CanVisit(this.Current, next, unvisited);
        }

        Current.IsVisited = true;
        unvisited.Remove(Current);
    }

    
    // Find the other options and call them with yourself, or should we be able to make a copy of the object?
    // Return true if succesfull
    public bool TryDown(MapPoint[,] map, out MapPoint next)
    {
        if (Current.Y + 1 < MaxY) {
            next = map[Current.X, Current.Y + 1];
            return true;
        }
        
        next = null;
        return false;
    }

    public bool TryUp(MapPoint[,] map, out MapPoint next)
    {
        if (Current.Y > 0) {
            next = map[Current.X, Current.Y - 1];
            return true;
        }
        
        next = null;
        return false;
    }

    public bool TryRight(MapPoint[,] map, out MapPoint next)
    {
        if (Current.X + 1 < MaxX) {
            next = map[Current.X + 1, Current.Y];
            return true;
        }
        
        next = null;
        return false;
    }

    public bool TryLeft(MapPoint[,] map, out MapPoint next)
    {
        if (Current.X > 0) {
            next = map[Current.X - 1, Current.Y];
            return true;
        }
        
        next = null;
        return false;
    }
}

public class MapPoint
{
    // The x and y is in the array so we don't care here? Or should we know our neighbours?
    // The score should be there, we should be able to compare them easily.
    public MapPoint(char c, int x, int y)
    {
        this.X = x;
        this.Y = y;
        if (c == 'S')
        {
            this.Height = 0;
            this.IsStart = true;
        }
        else if (c == 'E')
        {
            this.Height = CalcCharHeight('z') + 1;
            this.IsEnd = true;
        }
        else 
        {
            this.Height = CalcCharHeight(c);
        }   
    }

    private int CalcCharHeight(char c)
    {
        return (int)c - 'a' + 1;
    }

    public bool IsEnd { get; set; }
    public bool IsStart { get; set; }
    public bool IsVisited { get; set; }
    public uint TentativeDistanceValue { get; set; } = uint.MaxValue;
    public int Height {get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    
}

public class Day12
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day12.txt");
        var maxX = lines[0].Length;
        var maxY = lines.Length;

        // No let's parse it into a field double index :-)
        var allPoints = new HashSet<MapPoint>();
        var startPoints = new HashSet<MapPoint>();
        MapPoint[,] map = new MapPoint[maxX, maxY];

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                char c = lines[y][x];
                var newPoint = new MapPoint(c, x, y);
                map[x,y] = newPoint;
                allPoints.Add(newPoint);
                if (c == 'a')
                {
                    startPoints.Add(newPoint);
                }
            }
        }

        var allAnswers = startPoints.Select(s => FindNumberOfSteps(s, allPoints, map)).OrderBy(i => i).ToArray();
        System.Console.WriteLine($"part2: {allAnswers[0]}");     
    }

    public uint FindNumberOfSteps(MapPoint start, HashSet<MapPoint> allPoints, MapPoint[,] map)
    {
        foreach (var point in allPoints)
        {
            point.TentativeDistanceValue = uint.MaxValue;
            point.IsVisited = false;
        }

        start.TentativeDistanceValue = 0;
        var unvisited = allPoints.ToHashSet();
        return Explore(unvisited, map);
    }

    public uint Explore(HashSet<MapPoint> unvisited, MapPoint[,] map) {
        while(unvisited.Count > 0) {
            var closest = unvisited.OrderBy(u => u.TentativeDistanceValue).First();
            var closestExlorer = new Explorer(closest, map.GetLength(0), map.GetLength(1));
            
            if (closestExlorer.Current.IsEnd) 
            {
                return closest.TentativeDistanceValue;
            }

            closestExlorer.Visit(unvisited, map);
        }

        return 0;
    }
}
