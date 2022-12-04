namespace advent_of_code;

public class ElveGroup 
{
    public ElveGroup(Rucksack[] rucksacks)
    {
        _rucksacks = rucksacks.ToList();
    }

    public int FindBadge()
    {
        var result = _rucksacks[0].c.ToList();
        foreach (var rucksack in _rucksacks)
        {
            if (rucksack.c != result)  // Skip the first!
            {
                result = rucksack.c.Intersect(result).ToList();
            }
        }

        return result.Single();
    }

    List<Rucksack> _rucksacks;
}

public class Rucksack
{
    public static int CalculatePriority(char c) 
    {
        if (Char.IsUpper(c))
        {
            return (int)c - (int)'A' + 26 + 1;
        }
        else 
        {
            return (int)c - (int)'a' + 1;
        }
    }

    public Rucksack(string line)
    {
        var totalLength = line.Length;
        var halfLength = totalLength / 2;
        c = line.Select(c=>CalculatePriority(c)).ToList();
        c1 = line.Take(halfLength).Select(c=>CalculatePriority(c)).ToList();
        c2 = line.Skip(halfLength).Take(halfLength).Select(c=>CalculatePriority(c)).ToList();
    }

    public List<int> GetDoubleValues()
    {
        return c1.Intersect(c2).ToList();
    }

    public List<int> c;
    List<int> c1;
    List<int> c2;
}

public static class Day3 {

    public static async Task Execute()
    {
        var allLines = await File.ReadAllLinesAsync("day3.txt");
        var rucksacks = allLines.Select(l => new Rucksack(l)).ToList();
        var sums = rucksacks.Sum(r=> r.GetDoubleValues().Sum());

        var groups = rucksacks.Chunk(3)
            .Select(rs=>new ElveGroup(rs))
            .ToList();
        var badgeSum = groups.Sum(g=>g.FindBadge());
        System.Console.WriteLine(badgeSum);
    }

}