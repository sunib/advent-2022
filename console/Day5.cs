using System.Text.RegularExpressions;

namespace advent_of_code;

public record CraneInstruction
{
    public int Count { get; set; }
    public int From { get; set; }
    public int To { get; set; }
}

public class Day5
{
    Regex instructionParser;

    public Day5()
    {
        instructionParser = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.IgnoreCase);
    }

    // Position in string is 1 5 9 etc.
    // [G] [G] [G] [N] [V] [V] [T] [Q] [F]
    // 012345678901234
    private int GetCrateDigitPosition(int i)
    {
        return i * 4 + 1;
    }

    public List<Stack<char>> ParseHeader(string[] lines)
    {
        var result = new List<Stack<char>>();
        foreach (var line in lines.Reverse()) // We use a stack and want to push pop off course
        {
            for (int i = 0; GetCrateDigitPosition(i) < line.Length; i++)
            {
                if (result.Count == i)
                {    // Fill the list while going
                    result.Add(new Stack<char>());
                }

                char crate = line[GetCrateDigitPosition(i)];
                if (Char.IsAsciiLetterUpper(crate))
                {
                    result[i].Push(crate);
                }
            }
        }

        return result;
    }

    public CraneInstruction ParseInstruction(string line)
    {
        var matches = instructionParser.Match(line);
        if (matches.Groups.Count == 4)
        {
            return new CraneInstruction
            {
                Count = int.Parse(matches.Groups[1].Value),
                From = int.Parse(matches.Groups[2].Value) - 1,  // Zero-based please
                To = int.Parse(matches.Groups[3].Value) - 1 // Zero-based please
            };
        }
        else
        {
            throw new InvalidDataException("Invalid instruction");
        }
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("day5_6mb_large.txt");

        // Start parsing the rest
        var headerLines = lines.TakeWhile(s => s != string.Empty).ToArray();
        var stacks = ParseHeader(headerLines);

        var instructions = lines.Skip(headerLines.Length + 1)
            .Select(l => ParseInstruction(l))
            .ToArray();

        var toBeMoved = new Stack<char>();
        var executedInstructions = 0;
        foreach (var instruction in instructions)
        {
            // Answer 1
            while (instruction.Count-- > 0)
            {
                stacks[instruction.To].Push(
                    stacks[instruction.From].Pop());
            }

            // Answer 2
            // while (instruction.Count-- > 0)
            //     toBeMoved.Push(stacks[instruction.From].Pop());
            // while (toBeMoved.Count > 0)
            //     stacks[instruction.To].Push(toBeMoved.Pop());

            if (executedInstructions++ % 10000 == 0)
            {
                System.Console.WriteLine(((float)executedInstructions / (float)instructions.Length) * 100 + "%");
            }
        }

        System.Console.WriteLine("Answer: " + new String(stacks.Select(s => s.Peek()).ToArray()));
    }
}