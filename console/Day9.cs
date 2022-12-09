namespace advent_of_code_day9;

public interface IPoint {
    public int X { get; set; }
    public int Y { get; set; }
}

public class Head : IPoint
{
    public Head(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    // Return true if succesfull
    public void GoDown()
    {
        Y -= 1;
    }

    public void GoUp()
    {
        Y += 1;
    }

    public void GoRight()
    {
        X += 1;
    }

    public void GoLeft()
    {
        X -= 1;
    }
}

public class Knot : IPoint
{
    public Knot(int x, int y, IPoint followPoint)
    {
        FollowPoint = followPoint;
        X = x;
        Y = y;
    }

    public IPoint FollowPoint { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    // Want to follow head but we should get a signal that something changes
    private Tuple<int,int> CalculateDelta()
    {
        return new Tuple<int,int>(FollowPoint.X - X, FollowPoint.Y - Y);
    }

    private bool IsAdjacentOrSame(Tuple<int,int> delta)        // Is it close enough to the head?
    {
        if (delta.Item1 > 1 || delta.Item1 < -1) 
        {
            return false;
        }

        if (delta.Item2 > 1 || delta.Item2 < -1)
        {
            return false;
        }

        return true;
    }

    private int MaxToAbsOne(int value)
    {
        if (value > 1)
        {
            return 1;
        }

        if (value < -1)
        {
            return -1;
        }

        return value;
    }

    public void Follow()
    {
        // Move in such a way that the distance gets shorter
        // We should only do something if it's not adjacent at this moment!
        var delta = CalculateDelta();
        if (!IsAdjacentOrSame(delta)) 
        {
            X += MaxToAbsOne(delta.Item1);
            Y += MaxToAbsOne(delta.Item2);
        }
    }
}


public class Instruction
{
    public Instruction(string line)
    {
        var parts = line.Split(' ');
        switch (parts[0])
        {
            case "U":
                HeadAction = aw => aw.GoUp();
                break;
            case "D":
                HeadAction = aw => aw.GoDown();
                break;
            case "R":
                HeadAction = aw => aw.GoRight();
                break;
            case "L":
                HeadAction = aw => aw.GoLeft();
                break;
            default:
                throw new InvalidDataException("Unknown action");
        }
        
        Repeats = int.Parse(parts[1]);
    }

    public Action<Head> HeadAction { get; set; }

    public int Repeats { get; set; }
}

public class Day9
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day9.txt");
        var instructions = lines.Select(l => new Instruction(l)).ToArray();

        // Setup initial state
        var tailVisits = new HashSet<Tuple<int, int>>();
        var head = new Head(0, 0);

        var knots = new List<Knot>();
        IPoint tail = head;
        while (knots.Count < 9)
        {
            knots.Add(new Knot(0, 0, tail));
            tail = knots.Last();            
        }

        // Calculate all instructions and mark the places where the tail was
        foreach (var instruction in instructions)
        {
            while (instruction.Repeats-- > 0)
            {
                instruction.HeadAction(head);
                foreach (var knot in knots)
                {
                    knot.Follow();                    
                }
                
                tailVisits.Add(new Tuple<int, int>(tail.X, tail.Y));
            }
        }

        System.Console.WriteLine("tailPositions: " + tailVisits.Count());    
    }
}
