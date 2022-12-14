using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace advent_of_code_day13;


public class ComLine : IComparable
{
    public ComLine(string line, bool isDividerPacket = false)
    {
        Line = JsonConvert.DeserializeObject<JToken>(line);
        IsDividerPacket = isDividerPacket;
        RawLine = line;
    }

    public bool IsDividerPacket { get; set; }

    public string RawLine { get; set; } = "unknown";

    public ComLine(JToken line)
    {
        Line = line;
    }

    // Compare them
    // left == right returns 0
    // left > right returns 1
    // left < right returns -1
    public int CompareTo(object? rawRight)
    {
        var right = rawRight as ComLine;
        if (right == null)
          throw new InvalidProgramException("This should not happen in this example");

        // The right side is leading, see if the left side can follow
        if (this.Line is JValue && right.Line is JValue)
        {
            var valueLeft = this.Line.ToObject<int>();
            var valueRight = ((JToken)right.Line).ToObject<int>();
            
            return valueLeft.CompareTo(valueRight);
        }
        
        if (this.Line is JArray && right.Line is JArray)
        {
            var itLeft = ((JArray)this.Line).GetEnumerator();
            var itRight = ((JArray)right.Line).GetEnumerator();
            do 
            {
                var leftMoved = itLeft.MoveNext();
                var rightMoved = itRight.MoveNext();                

                if (leftMoved && rightMoved)
                {
                    var left = new ComLine(itLeft.Current);
                    var compare = left.CompareTo(new ComLine(itRight.Current));
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
        return ConvertIfNeeded(this.Line)
            .CompareTo(
                ConvertIfNeeded(right.Line));
    }

    public ComLine ConvertIfNeeded(JToken container)
    {
        var result = container as JArray;
        if (result == null)
        {
            // We just pick one here is that right? Or should we be greedy until you have the same number of elements?
            result = new JArray(new Object[] { container } );
        }

        return new ComLine(result);
    }

    public JToken Line { get; set; }
}

public class Day13
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day13.txt");
        var parsedLines = lines
            .Where(s => s != string.Empty)
            .Select(s => new ComLine(s))
            .ToList();        
        parsedLines.AddRange(new ComLine[] { 
            new ComLine("[[2]]", true), 
            new ComLine("[[6]]", true)});

        var counter = 0;
        var ordered = parsedLines
            .OrderBy(c => c)
            .Select(i => new {index = ++counter, line = i })
            .ToList();

        var dividerPackets = ordered.Where(l => l.line.IsDividerPacket).ToList();
        System.Console.WriteLine($"part2: {dividerPackets[0].index * dividerPackets[1].index}");     
    }
}
