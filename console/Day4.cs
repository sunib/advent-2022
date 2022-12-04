using System.Text.RegularExpressions;

namespace advent_of_code;

public class Range
{
    public Range(string begin, string end)
    {
        Begin = int.Parse(begin);
        End = int.Parse(end);
    }

    public int Begin { get; set; }
    public int End { get; set; }

    // .234... (us)
    // .23.... (other)
    // Should return true
    public bool IsWithin(Range other)
    {
        return other.Begin >= Begin && other.End <= End;
    }

    public bool Overlap(Range other)
    {
        // return
        //     (other.Begin >= Begin && other.Begin <= End) ||
        //     (other.End >= Begin && other.End <= End) ||
        //     (Begin >= other.Begin && Begin <= other.End) ||
        //     (End >= other.Begin && End <= other.End);
        // Works exactly the same but shorter: https://gathering.tweakers.net/forum/list_messages/2160536/7#73684330
        return End >= other.Begin && other.End >= Begin;
    }
}

public class ElvePair
{
    public ElvePair(Range first, Range second)
    {
        First = first;
        Second = second;
    }

    public Range First { get; set; }
    public Range Second { get; set; }

    public bool Overlap() {
        return First.Overlap(Second);
    }

    public bool FullyContains()
    {
        return First.IsWithin(Second) || Second.IsWithin(First);
    }
}

public class Day4
{
    Regex r;
    public Day4()
    {
        string pat = @"(\d*)-(\d*),(\d*)-(\d*)";
        r = new Regex(pat, RegexOptions.IgnoreCase);
    }

    public ElvePair ParseLine(string line)
    {    
        Match m = r.Match(line);
        if (m.Groups.Count == 5)
            return new ElvePair(
                new Range(m.Groups[1].Value, m.Groups[2].Value), 
                new Range(m.Groups[3].Value, m.Groups[4].Value));
        else 
            throw new InvalidDataException("Unexpected number of groups parsed on line");
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("day4.txt");
        var elvePairs = lines.Select(l => ParseLine(l)).ToList();
        var answer = elvePairs.Count(ep => ep.FullyContains());
        System.Console.WriteLine(answer);
        var answer2 = elvePairs.Count(ep => ep.Overlap());
        System.Console.WriteLine(answer2);
    }
}