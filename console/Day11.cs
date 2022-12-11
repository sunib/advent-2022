using System.Numerics;
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
        this.Items = result.Groups["items"].Value.Split(",").Where(s => int.TryParse(s, out var dontcare)).Select(n=>new BigInteger(int.Parse(n))).ToList();
        this.Operand = result.Groups["operand"].Value;
        this.Value = result.Groups["value"].Value;
        this.TestDivisor = int.Parse(result.Groups["divisor"].Value);
        this.MonkeyTrue = int.Parse(result.Groups["monkeyTrue"].Value);
        this.MonkeyFalse = int.Parse(result.Groups["monkeyFalse"].Value);
        
    }
    
    public List<BigInteger> Items { get; set; }        // The value is the worry level for the 'me'
    public string Operand { get; set; }
    public string Value { get; set; }
    public int TestDivisor { get; set; }
    public int MonkeyTrue { get; set; }
    public int MonkeyFalse { get; set; }

    public BigInteger Inspections { get; set; } = 0;

    public Monkey Inspect(Monkey[] monkeys, int damageControlFactor)
    {
        // Inspect all items in order and clear them
        foreach (var item in Items)
        {
            BigInteger newValue = 0;
            if (!BigInteger.TryParse(Value, out var value))
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

            newValue %= damageControlFactor;
            // De elende wordt altijd groter
            // Het zijn allemaal priemgetallen, dan is het dus nooit deelbaar behalve door zichzelf
            // Moeten we bij elk aapje opslaan welk getal ze al gehad hebben?


            //newValue /= 3;
            BigInteger rest = newValue % (BigInteger)this.TestDivisor;
            if (rest == 0)    // It's all about this dicision, we don't care for the end answer about all the digits. So we might just want to save the divisor -> priemgetal!
            {
                monkeys[MonkeyTrue].Items.Add(newValue);
            }
            else 
            {
                monkeys[MonkeyFalse].Items.Add(newValue);   // Moeten we alle resten zelf gaan bijhouden? Maar de +6 dan?
            }
        }

        Inspections = Inspections + Items.Count;
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

        // The first round
        var rounds = 10000;
        while (rounds-- > 0)
        {
            monkeys = monkeys.Select(m => m.Inspect(monkeys, damageControlFactor)).ToArray();
            System.Console.WriteLine($"{rounds}");
        }

        var orderedMonkeys = monkeys.OrderByDescending(m=>m.Inspections).ToArray();
        System.Console.WriteLine($"part1: { orderedMonkeys[0].Inspections * orderedMonkeys[1].Inspections}" );    

        // Answer is not correct, its to low
        // 11521878962
        // 11684352498  // Answer is also to low
        // 12264233511  // Answer is still to low but now we don't get our overflow!
        // 32394240247  // This is also not right! Now wait 5 minutes

        // Go back to the 'small' file:
        // For this file it should be:
        // 2713310158
        // 2963931363
        
        // 9999200007
        //2713310158
    }
}
