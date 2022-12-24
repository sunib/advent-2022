using System.Diagnostics;

namespace Helpers.BreadthFirstSearch; 

public interface INode {
    
    public string Id { get; set; }
    public HashSet<T> GetAdjacenctNodes<T>() where T : INode;
}

public class BreadthFirstSearch<T> where T : class, INode
{

    Dictionary<T,
        Func<T, IEnumerable<T>>> traversals = new Dictionary<T, Func<T, IEnumerable<T>>>();

    public Dictionary<string, int> searchCache = new Dictionary<string, int>();
    public int FindPath(T start, T end)
    {
        var cacheString = $"{start.Id}{end.Id}";
        
        int result = 0;
        if (!searchCache.TryGetValue(cacheString, out result))
        {
            Func<T, IEnumerable<T>> lookupFunction;
            if (!traversals.TryGetValue(start, out lookupFunction))
            {
                lookupFunction = Search(start);
                traversals.Add(start, lookupFunction);
            }

            var path = lookupFunction(end);
            result = path.Count() - 1;
            searchCache.Add(cacheString, result);
        }

        return result;
    }


    // Thanks koder dojo
    // https://www.koderdojo.com/blog/breadth-first-search-and-shortest-path-in-csharp-and-net-core
    public Func<T, IEnumerable<T>> Search(T start) {
        var sw = new Stopwatch();
        sw.Start();
        var previous = new Dictionary<T, T>();
        var queue = new Queue<T>();
        queue.Enqueue(start);
        while (queue.Count > 0) {
            var vertex = queue.Dequeue();            
            foreach(var neighbor in vertex.GetAdjacenctNodes<T>())
            {
                if (previous.ContainsKey(neighbor))
                    continue;
    
                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        // So you do that once and then are able to do it again?
        Func<T, IEnumerable<T>> shortestPath = (v) => {
            var path = new List<T>{};

            var current = v;
            while (!current.Equals(start)) {
                path.Add(current);
                current = previous[current];
            };

            path.Add(start);
            path.Reverse();
            return path;
        };

        System.Console.WriteLine($"Completed BFS in {sw.Elapsed}");
        return shortestPath;
    }
}
