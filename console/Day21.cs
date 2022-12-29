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

    private static double humn;

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
            if (Id == "humn")
                return humn;
        
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

        public string Id { get; set; }
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
        
        // humn
        // root should be = so the numbers should match.
        // root: fglq (this is where x is found at > 20 levels deep) + fzbp (42130890593816)
        // Which one is actually using humn?                                 42130857978819,95
        //                                                                   42130857978813,234
        //                                                                   42130857978829,53
            //                                                               42130857981875,93
            //                                                               42130857978617,76
        var expression = operations["fglq"];  // Can we get back the path back to the top?
        
        // humn fglq
        // 0    100576930890443,2
        // 1    100576930890426,9
        // 2    100576930890410,62
        // 3    100576930890394,33
        // 100  100576930888814,11
        // 200  100576930887185,02
        // 1000 100576930874152,3
        // -16,3 per stap
        // 3587649564887 42130857978829,53
        // 3587649564000 3587649564887,5874
        // 3587649564700 42130857981875,93
        // 3587649564900 42130857978617,76
        // 3587649566000 3587649564887,5874
        double fcbp = 100576930890443.2 - 42130890593816;
        double delta = 16.2909;
        double answer = fcbp / delta;
        System.Console.WriteLine(answer);

        humn = 3587649564900;
        for (double i = 3587647562800; i < 3587647562863; i++)
                     // 3587647562860
//3587647562851
        {
            humn = i;   // Brute forcing the answer by approximation
            var value2 = expression.Evaluate(operations);
            var diff = value2 - 42130890593816;
            if (diff < 0)
            {
                System.Console.WriteLine("Flip!");
            }
            System.Console.WriteLine($"part2: {value2 - 42130890593816}");

        }


        humn = 3587647562851;
        var value = expression.Evaluate(operations);
        System.Console.WriteLine($"part2 - final: {value - 42130890593816}");
    }
}
