namespace advent_of_code;

public class AwareCoordinate 
{
    public AwareCoordinate(int x, int y, int maxX, int maxY)
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

public class Day8
{
    
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day8.txt");
        var maxX = lines[0].Length;
        var maxY = lines.Length;

        // No let's parse it into a field double index :-)
        int[,] heights = new int[maxX, maxY];
        int[,] vissible = new int[maxX, maxY];   // Set to 1 if visible from any side! Then afterwards we can count the ones
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxY; x++)
            {
                heights[x,y] = int.Parse(lines[y][x].ToString());
            }
        }


        for (int x = 0; x < maxX; x++)
        {
            // Look from top to bottom
            var pos = new AwareCoordinate(x, 0, maxX, maxY);
            PlayLightParticle(pos, -1, heights, vissible, (aw) => aw.GoDown());

            // Look from bottom to top
            pos = new AwareCoordinate(x, maxY - 1, maxX, maxY);
            PlayLightParticle(pos, -1, heights, vissible, (aw) => aw.GoUp());
        }

        for (int y = 0; y < heights.GetLength(1); y++)
        {   
            // Look from left to right
            var pos = new AwareCoordinate(0, y, maxX, maxY);
            PlayLightParticle(pos, -1, heights, vissible, (aw) => aw.GoRight());

            // Look from right to left
            pos = new AwareCoordinate(maxX - 1, y, maxX, maxY);
            PlayLightParticle(pos, -1, heights, vissible, (aw) => aw.GoLeft());
        }

        // Now count how many ones we have in our visible list.
        var visibleTrees = 0;
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxY; x++)
            {
                if (vissible[x,y] == 1)
                    visibleTrees++;
            }
        }

        System.Console.WriteLine("Number of visible trees: " + visibleTrees);
    
        // Part 2
        // Get to each tree and calculate how many trees it can see. Pick the 
        var example1 = CalculateMultipliedSCore(2, 1, maxX, maxY, heights);
        var example2 = CalculateMultipliedSCore(2, 3, maxX, maxY, heights);

        int highestScenicScore = 0;
        for (int y = 1; y < (maxY - 1); y++)
        {
            for (int x = 1; x < (maxX - 1); x++)
            {
                int score = CalculateMultipliedSCore(x, y, maxX, maxY, heights);
                if (score > highestScenicScore)
                {
                    highestScenicScore = score;
                }
            }
        }

        System.Console.WriteLine("Highest score: " + highestScenicScore);
    
    }

    public int CalculateMultipliedSCore(int x, int y, int maxX, int maxY, int[,] heights)
    {
        var pos = new AwareCoordinate(x, y, maxX, maxY);
        var d1 = CalculateScenicScore(pos, heights[x,y], 0, heights, aw => aw.GoUp());
        pos = new AwareCoordinate(x, y, maxX, maxY);
        var d2 = CalculateScenicScore(pos, heights[x,y], 0, heights, aw => aw.GoLeft());
        pos = new AwareCoordinate(x, y, maxX, maxY);
        var d3 = CalculateScenicScore(pos, heights[x,y], 0, heights, aw => aw.GoRight());
        pos = new AwareCoordinate(x, y, maxX, maxY);
        var d4 = CalculateScenicScore(pos, heights[x,y], 0, heights, aw => aw.GoDown());
        return d1*d2*d3*d4;
    }


    public int CalculateScenicScore(AwareCoordinate pos, int maxHeight, int depth, int[,] heights, Func<AwareCoordinate, bool> next)
    {
        int result = depth;
        if (heights[pos.X, pos.Y] < maxHeight || result == 0)
        {
            if (next(pos))
            {
                result++;
                result = CalculateScenicScore(pos, maxHeight, result, heights, next);
            }
        }

        return result;
    }

    public void PlayLightParticle(AwareCoordinate pos, int currentHeight, int[,] heights, int[,] vissible, Func<AwareCoordinate, bool> next)
    {
        // We come in at a edge
        int treeHeight = heights[pos.X,pos.Y];
        if (currentHeight < 9) // no use to continue if it's already at heighest
        {
            if (currentHeight < treeHeight) // Can we see the tree?
            {
                currentHeight = treeHeight;
                vissible[pos.X,pos.Y] = 1;    
            }

            if (next(pos))
                PlayLightParticle(pos, currentHeight, heights, vissible, next);
        }
    }
}
