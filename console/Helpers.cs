namespace Helpers;

// https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp

public interface IWeightedGraph<T> where T : ILocation
{
    double Cost(T a, T b);
    IEnumerable<T> Neighbors(T id);
}

public interface ILocation
{
    int X { get; }
    int Y { get; }
    double CalculateCost(ILocation neighbor);
    bool IsPassable(ILocation neighbor);
}

public class SquareGrid<T> : IWeightedGraph<T> where T : ILocation
{
    public T[,] Locations { get; }
    public int Width { get; }
    public int Height { get; }

    private record Direction(int X, int Y);

    private static readonly Direction[] Directions = 
    {
        new (1, 0),
        new (0, -1),
        new (-1, 0),
        new (0, 1)
    };

    public SquareGrid(T[,] locations)
    {
        Locations = locations;
        Height = Locations.GetLength(0);
        Width = Locations.GetLength(1);
    }

    public bool Inbounds(int x, int y) => 0 <= x && x < Width && 0 <= y && y < Height;

    public bool Passable(T a, T b)
    {
        return a.IsPassable(b);
    }
    
    public double Cost(T a, T b)
    {
        return a.CalculateCost(b);
    }

    public IEnumerable<T> Neighbors(T id)
    {
        foreach (var dir in Directions)
        {
            var nextX = id.X + dir.X;
            var nextY = id.Y + dir.Y;
            if (!Inbounds(nextX, nextY)) continue;
            
            var next = Locations[nextY, nextX];
            if (Passable(id, next)) yield return next;
        }
    }
}

public class AStarSearch<T> where T : ILocation
{
    public Dictionary<T, T> CameFrom { get; } = new();
    public Dictionary<T, double> CostSoFar { get; } = new();

    public T? FindGoal(IWeightedGraph<T> graph, T start, T goal)
    {
        CameFrom.Clear();
        CostSoFar.Clear();
        
        var pq = new PriorityQueue<T, double>();
        pq.Enqueue(start, 0);
        
        CameFrom[start] = start;
        CostSoFar[start] = 0;

        return TryFindGoal(pq, graph, goal);
    }
    
    public T? FindGoal(IWeightedGraph<T> graph, IEnumerable<T> starts, T goal)
    {
        CameFrom.Clear();
        CostSoFar.Clear();

        var pq = new PriorityQueue<T, double>();
        foreach (var start in starts)
        {
            pq.Enqueue(start, 0);

            CameFrom[start] = start;
            CostSoFar[start] = 0;
        }

        return TryFindGoal(pq, graph, goal);
    }

    private T? TryFindGoal(PriorityQueue<T, double> pq, IWeightedGraph<T> graph,
        T goal)
    {
        while (pq.Count > 0)
        {
            var current = pq.Dequeue();

            if (current.Equals(goal))
            {
                return current;
            }

            foreach (var next in graph.Neighbors(current))
            {
                var newCost = CostSoFar[current] + graph.Cost(current, next);
                if (CostSoFar.ContainsKey(next) && !(newCost < CostSoFar[next])) continue;
                
                CostSoFar[next] = newCost;
                var priority = newCost + Heuristic(next, goal);
                pq.Enqueue(next, priority);
                CameFrom[next] = current;
            }
        }

        return default;
    }

    private static double Heuristic(ILocation a, ILocation b)
    {
        // Hueristic is important for beeing fast: but how do yuo do that in a non grid thing?
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
