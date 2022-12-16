using System.Text.RegularExpressions;
namespace advent_of_code_day14;

public class Day14
{
    public class Wall
    {
        public Wall(string line, Regex r)
        {
            MatchCollection matches = r.Matches(line);
            foreach (Match m in matches)
            {
                this.points.Add(
                    new Point(
                        int.Parse(m.Groups["x"].Value),
                        int.Parse(m.Groups["y"].Value)));
            }

            MaxX = this.points.Max(p => p.X);
            MaxY = this.points.Max(p => p.Y);
        }

        public List<Point> points = new List<Point>();
        public int MaxX { get; set; }
        public int MaxY { get; set; }
    }

    public class Point
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    Regex r;

    public Day14()
    {
        string pat = @"(?<x>\d*),(?<y>\d*)";
        r = new Regex(pat, RegexOptions.Compiled);
    }


    public enum Fill
    {
        air = 0,    //.
        rock = '#',   //#
        sand = '0',   //0 (at rest)
        source = '+'  //+ (source)
    }

    public class MovingSand 
    {
        public MovingSand(int x, int y, int maxY)
        {
            X = x;
            Y = y;
            MaxY = maxY;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int MaxY { get; set; }

        public bool EthernalFall { get; set; }
        
        // Return true is fail succeeded, when it's false we can stop. We should also check if is below the MaxY!
        public bool Fall(Fill[,] grid)
        {
            if (Y >= MaxY)
            {
                EthernalFall = true;
                return false;
            }

            // Go one down if you can.
            if (grid[X,Y+1] == Fill.air)
            {
                Y++;
                return true;
            }
            else 
            {
                // Check if we can go left or right
                if (grid[X-1,Y+1] == Fill.air)
                {
                    X--;
                    Y++;
                    return true;
                }
                else if (grid[X+1,Y+1] == Fill.air)
                {
                    X++;
                    Y++;
                    return true;
                }
            }

            return false;
        }
    }

    // Always make sure that the smallest is first
    public Tuple<int,int> SmallestFirst(int n1, int n2)
    {
        if (n1 > n2)
            return new Tuple<int,int>(n2, n1);
        else
            return new Tuple<int,int>(n1, n2);
    }


    public async Task Execute()
    {
        System.Console.WriteLine("Let's start it");
        var lines = await File.ReadAllLinesAsync("console/day14.txt");
        var walls = lines.Select(l => new Wall(l, r)).ToList();
        var maxX = walls.Max(w => w.MaxX);
        var maxY = walls.Max(w => w.MaxY);

        var grid = new Fill[maxX + 5, maxY + 5];

        // Fill all the walls
        foreach (var wall in walls)
        {
            // Decide which side to walk
            Point previous = null;
            foreach (var current in wall.points)
            {
                // If we have a previous then we should walk to the current
                if (previous != null)
                {
                    if (previous.X != current.X && previous.Y == current.Y)
                    {
                        var loc = SmallestFirst(previous.X, current.X);
                        for (int x = loc.Item1; x < loc.Item2; x++)
                        {
                            grid[x, current.Y] = Fill.rock;
                        }
                    }
                    else if (previous.X == current.X && previous.Y != current.Y)
                    {
                        var loc = SmallestFirst(previous.Y, current.Y);
                        for (int y = loc.Item1; y < loc.Item2; y++)
                        {
                            grid[current.X, y] = Fill.rock;
                        }
                    }
                    else 
                    {
                        throw new InvalidDataException("Didnt expect diagnoal walls");
                    }
                }

                grid[current.X, current.Y] = Fill.rock;     // If only one point is known it's still a point that needs to be drawn.
                previous = current;
            }
        }

        

        // Set the source
        grid[500,0] = Fill.source;
        DrawGrid(grid);

        int sands = 0;

        MovingSand movingSand = null;
        do {
            movingSand = new MovingSand(500, 0, maxY);
            while(movingSand.Fall(grid));
            if (!movingSand.EthernalFall) {
                sands++;
                grid[movingSand.X, movingSand.Y] = Fill.sand;
            }

            DrawGrid(grid);
        } while (!movingSand.EthernalFall);

        System.Console.WriteLine($"part1: {sands}");
    }

    public void DrawGrid(Fill[,] grid)
    {
        int minX = grid.GetLength(0);
        int minY = grid.GetLength(1);
        int maxX = 0;
        int maxY = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x,y] != Fill.air)
                {
                    if (minX > x)
                    {
                        minX = x;
                    }

                    if (minY > y)
                    {
                        minY = y;
                    }

                    if (maxX < x)
                    {
                        maxX = x;
                    }

                    if (maxY < y)
                    {
                        maxY = y;
                    }
                }
            }
        }

        for (int y = minY; y <= maxY; y++)
        {
            Console.Write(y.ToString("D4") + " ");

            for (int x = minX; x <= maxX; x++)
            {
                char c = '.';
                if (grid[x,y] != 0)
                {
                    c = (char)grid[x,y];
                }
                
                Console.Write(c);
            }

            Console.Write(Environment.NewLine);
        }
    }
}
