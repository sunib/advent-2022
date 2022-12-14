using System.Text.RegularExpressions;
namespace advent_of_code_day11;

public class Monkey
{
    public Monkey(string[] unparsedLines)
    {
        var input = new string[] {
            @"Monkey \d*:",
            @"  Starting items: (?<items>.*)",
            @"  Operation: new = old (?<operand>.) (?<value>.*)",
            @"  Test: divisible by (?<divisor>\d*)",
            @"    If true: throw to monkey (?<monkeyTrue>\d*)",
            @"    If false: throw to monkey (?<monkeyFalse>\d*)"
        };
            
        var inputLine = String.Join(Environment.NewLine, input);
        var monkeyParser = new Regex(inputLine, RegexOptions.Multiline);
        var oneline = String.Join(Environment.NewLine, unparsedLines);
        var result = monkeyParser.Match(oneline);
        this.Items = result.Groups["items"].Value.Split(",").Where(s => ulong.TryParse(s, out var dontcare)).Select(n => ulong.Parse(n)).ToList();
        this.Operand = result.Groups["operand"].Value;
        this.Value = result.Groups["value"].Value;
        this.TestDivisor = int.Parse(result.Groups["divisor"].Value);
        this.MonkeyTrue = int.Parse(result.Groups["monkeyTrue"].Value);
        this.MonkeyFalse = int.Parse(result.Groups["monkeyFalse"].Value);
        
    }
    
    public List<ulong> Items { get; set; }        // The value is the worry level for the 'me'
    public string Operand { get; set; }
    public string Value { get; set; }
    public int TestDivisor { get; set; }
    public int MonkeyTrue { get; set; }
    public int MonkeyFalse { get; set; }

    public ulong Inspections { get; set; } = 0;

    public Monkey Inspect(Monkey[] monkeys, int damageControlFactor)
    {
        // Inspect all items in order and clear them
        foreach (var item in Items)
        {
            ulong newValue = 0;
            if (!ulong.TryParse(Value, out var value))
                Operand = "^";   // It's 'old' so multiply with yourself (^2)

            checked 
            {
                switch (this.Operand)
                {
                    case "^":
                        newValue = item * item;
                        break;
                    case "*":
                        newValue = item * value;
                        break;
                    case "+":
                        newValue = item + value;
                        break;
                }
            }

            newValue %= (ulong)damageControlFactor;
            ulong rest = newValue % (ulong)this.TestDivisor;
            if (rest == 0)
            {
                monkeys[MonkeyTrue].Items.Add(newValue);
            }
            else 
            {
                monkeys[MonkeyFalse].Items.Add(newValue);
            }
        }

        Inspections = Inspections + (ulong)Items.Count;
        Items.Clear();
        return this;
    }
}



public class Day11
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day11.txt");
        var monkeys = lines.Chunk(7)
            .Select(um => new Monkey(um))
            .ToArray();

        var damageControlFactor = monkeys.Aggregate(1, (result, monkey) => result *= monkey.TestDivisor);
        for (int rounds = 0; rounds < 10000; rounds++)
            monkeys = monkeys.Select(m => m.Inspect(monkeys, damageControlFactor)).ToArray();

        var orderedMonkeys = monkeys.OrderByDescending(m=>m.Inspections).ToArray();
        System.Console.WriteLine($"part2: { orderedMonkeys[0].Inspections * orderedMonkeys[1].Inspections}" );    
    }
}
