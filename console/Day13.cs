using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace advent_of_code_day13;


public class ComPair
{
    public ComPair(string[] pairs)
    {
        FirstString = pairs[0];
        SecondString = pairs[1];
        First = JsonConvert.DeserializeObject<JToken>(FirstString);
        Second = JsonConvert.DeserializeObject<JToken>(SecondString);
    }

    // Compare 
    // left == right returns 0
    // left > right returns 1
    // left < right returns -1
    public int IsInRightOrder(JToken left, JToken right) 
    {
        // The right side is leading, see if the left side can follow
        if (left is JValue && right is JValue)
        {
            var valueLeft = ((JToken)left).ToObject<int>();
            var valueRight = ((JToken)right).ToObject<int>();
            
            return valueLeft.CompareTo(valueRight);
            
        }
        
        if (left is JArray && right is JArray)
        {
            var itLeft = ((JArray)left).GetEnumerator();
            var itRight = ((JArray)right).GetEnumerator();
            do 
            {
                var leftMoved = itLeft.MoveNext();
                var rightMoved = itRight.MoveNext();                

                if (leftMoved && rightMoved)
                {
                    var compare = IsInRightOrder(itLeft.Current, itRight.Current);
                    if (compare != 0)
                    {
                        if (compare > 0)
                        {
                            return 1;
                        }
                        else 
                        {
                            return -1;
                        }
                    }                    
                }
                else if (!leftMoved && !rightMoved)
                {
                    // Both done so now what, then it's probably ok
                    return 0;
                }
                else if (!leftMoved)
                {
                    // One of them is depleted
                    return -1;
                }
                else if (!rightMoved)
                {
                    return 1;
                }

            } while (true);            
        }
        
        // One of them is not the same so we need to convert it in this case.
        // We can only have this last because it would go on forever otherwise.
        return IsInRightOrder(ConvertIfNeeded(left), ConvertIfNeeded(right));
    }

    public JArray ConvertIfNeeded(JToken container)
    {
        var result = container as JArray;
        if (result == null)
        {
            // We just pick one here is that right? Or should we be greedy until you have the same number of elements?
            return new JArray(new Object[] { container } );
        }

        return result;
    }

    public string FirstString { get; set; }
    public string SecondString { get; set; }
    public JToken First { get; set; }
    public JToken Second { get; set; }
}

public class Day13
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day13.txt");

        var pairs = lines.Where(s => s != string.Empty).Chunk(2).Select(s2 => new ComPair(s2)).ToList();
        int counter = 0;
        var results = pairs.Select(
            i => new { 
                someindex = ++counter,
                pairs = i, 
                compare = i.IsInRightOrder(i.First, i.Second) 
            }).ToArray();
        var sum = results.Where(a => a.compare == -1).Sum(a => a.someindex);
        System.Console.WriteLine($"part1: {sum}");     
    }

}
