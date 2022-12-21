using System.Numerics;
using System.Text.RegularExpressions;
namespace advent_of_code_day21;

public class Day21
{
    
    public interface IExpression
    {
        public string Id { get; set; }

        public double? Evaluate(Dictionary<string, IExpression> lookupTable); // Can we come up with a number? Or is to early? We can actually execute this on the whole mix.

    }

    public class Constant : IExpression
    {
        public Constant(string id, double value)
        {
            Id = id;
            Value = value;
        }
        public string Id {get; set;}

        public double Value { get; set; }

        public double? Evaluate(Dictionary<string, IExpression> lookupTable)
        {
            return Value;
        }
    }

    public class Operation : IExpression
    {
        public Operation(string id, string left, string op, string right)
        {
            Id = id;
            Left = left;
            Right = right;
            Op = op;
        }

        public string Id {get;set;}
        public string Left { get; set; }
        public string Right { get; set; }
        public string Op { get; set; }
        public double? Evaluate(Dictionary<string, IExpression> lookupTable)
        {
            var nr1 = lookupTable[Left].Evaluate(lookupTable);
            var nr2 = lookupTable[Right].Evaluate(lookupTable);
            if (nr1.HasValue && nr2.HasValue)
            {
                if (Op == "+")
                    return nr1 + nr2;
                else if (Op == "-")
                    return nr1 - nr2;
                else if (Op == "*")
                    return nr1 * nr2;
                else if (Op == "/")
                    return nr1 / nr2;
            }

            return null;
        }
    }

    public IExpression ParseLine(string line, Regex r)
    {
        var match = r.Match(line);
        string id = match.Groups["i"].Value;
        if (match.Groups["a"].Success)
        {
            return new Operation(id, match.Groups["a"].Value, match.Groups["b"].Value, match.Groups["c"].Value);
        }
        else if (match.Groups["d"].Success)
        {
            return new Constant(id, double.Parse(match.Groups["d"].Value));
        }

        throw new InvalidDataException("Should parse it all");
    }

    Regex r;

    public Day21()
    {
        string pat = @"(?<i>\w{4}): (((?<a>\w{4}) (?<b>.+) (?<c>\w{4}))|(?<d>\d+))";
        r = new Regex(pat, RegexOptions.Compiled);
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day21.txt");
        var operations = lines.Select(l => ParseLine(l,r)).ToDictionary(e => e.Id);
        // foreach (var item in operations)
        // {
        //     // We should create a new dictioanry perhaps?
        //     if (item.Value is Operation)
        //     {
        //         var newValue = item.Value.Evaluate(operations);
        //         if (newValue.HasValue)
        //         {
        //             operations[item.Key] = new Constant(item.Key, newValue.Value);
        //             System.Console.WriteLine("Yes replacing something!");
        //         }
        //     }            
        // }
        
        System.Console.WriteLine($"part1: {operations["root"].Evaluate(operations)}");
    }

}
