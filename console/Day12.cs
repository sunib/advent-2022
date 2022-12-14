namespace advent_of_code_day12;

public class Explorer 
{
    public Explorer(int x, int y, int maxX, int maxY)
    {
        X = x;
        Y = y;
        MaxX = maxX;
        MaxY = maxY;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public int MaxX { get; set; }
    public int MaxY { get; set; }

    public List<Tuple<int, int>> History {get;set;} = new List<Tuple<int,int>>();

    public bool CheckDestination(MapPoint current, MapPoint destination)
    {
        var diff = destination.Height - current.Height;
        if (diff <= 1 && !destination.IsVisited)  //&& diff >= -1
        {    
            return true;
        }
        
        return false;
    }

    public bool AreWeDone(MapPoint[,] map)
    {
        var currentPoint = map[this.X,this.Y];
        return currentPoint.IsEnd;
    }

    public IEnumerable<Explorer> LookArround(MapPoint[,] map)
    {
        // What are the options here?
        // Only 1 lower or 1 heigher and off course need to stay on the map
        // Also check if we have been there
        var currentPoint = map[this.X,this.Y];
        currentPoint.IsVisited = true;
        var test = (Explorer)this.MemberwiseClone();
        if (test.GoUp() && CheckDestination(currentPoint, map[test.X,test.Y]))
        {
            yield return test;
        }

        test = (Explorer)this.MemberwiseClone();
        if (test.GoDown() && CheckDestination(currentPoint, map[test.X,test.Y]))
        {
            yield return test;
        }

        test = (Explorer)this.MemberwiseClone();
        if (test.GoLeft() && CheckDestination(currentPoint, map[test.X,test.Y]))
        {
            yield return test;
        }

        test = (Explorer)this.MemberwiseClone();
        if (test.GoRight() && CheckDestination(currentPoint, map[test.X,test.Y]))
        {
            yield return test;
        }
    }

    
    // Find the other options and call them with yourself, or should we be able to make a copy of the object?
    // Return true if succesfull
    public bool GoDown()
    {
        Y += 1;
        return (Y < MaxY);
    }

    public bool GoUp()
    {
        Y -= 1;
        return (Y >= 0);
    }

    public bool GoRight()
    {
        X += 1;
        return (X < MaxX);
    }

    public bool GoLeft()
    {
        X -= 1;
        return (X >= 0);
    }
}

public class MapPoint
{
    // The x and y is in the array so we don't care here? Or should we know our neighbours?
    // The score should be there, we should be able to compare them easily.
    public MapPoint(char c)
    {
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

    public int CalcCharHeight(char c)
    {
        return (int)c - 'a' + 1;
    }

    public bool IsEnd { get; set; }
    public bool IsStart { get; set; }
    public bool IsVisited { get; set; }
    public int Height {get; set; }
    
}

public class Day12
{
    
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day12-simple.txt");
        var maxX = lines[0].Length;
        var maxY = lines.Length;

        // No let's parse it into a field double index :-)
        Explorer firstExplorer = null;
        MapPoint[,] map = new MapPoint[maxX, maxY];
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                var newPoint = new MapPoint(lines[y][x]);
                map[x,y] = newPoint;
                if (newPoint.IsStart) {
                    firstExplorer = new Explorer(x, y, maxX, maxY);
                }
            }
        }

        if (firstExplorer != null){
            var shortestPath = Explore(firstExplorer, map);
            System.Console.WriteLine($"Answer 1: { shortestPath }");
        }

        // We need recursion anyhow, so we will need to keep some stat on the points (where have we been what is shortest path back). The first one wins, how does that exactly work?
        // First see where you can go: then go and keep a pointer where you are going. This is only used for the single way back.

        // Do we need a walk arround object that is going to keep a stack with previous locations? What is the actual answer?
    
    }

    public int Explore(Explorer explorer, MapPoint[,] map) {
        if (explorer.AreWeDone(map))
        {
            return 0;
        }

        // Now we go through the grid in all possible ways!
        var shortestRoutes = explorer.LookArround(map)
            .Select(e => Explore(e,map))
            .Where(i => i >= 0)
            .OrderBy(i => i)
            .ToArray();
        
        var shortest = shortestRoutes.FirstOrDefault(-1);
        if (shortest != -1)
            shortest++;
        return shortest;
    }
}
