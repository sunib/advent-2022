using System.Numerics;
using System.Text.RegularExpressions;
namespace advent_of_code_day25;

public class Day25
{
    public char ConvertCharBack(int i)
    {
        if (i == -1)
            return '-';
        if (i == -2)
            return '=';
        
        return Convert.ToChar('0' + i);
    }

    /*
    2, 1, 0, minus (written -), and double-minus (written =). 
    Minus is worth -1, and double-minus is worth -2."
    */
    public int ConvertChar(char c)
    {
        int result = 0;
        if (char.IsNumber(c))
            result = c - '0';
        if (c == '-')
            result = -1;
        if (c == '=')
            result = -2;
        
        return result;
    }

    public Double ParseLine(string line)
    {
        Double result = 0;
        int length = line.Length - 1;
        foreach (var item in line)
        {
            result += (Double)ConvertChar(item) * (Double)Math.Pow(5, length);
            length--;
        }

        return result;
    }

    public string ToSnafu(double input)
    {
        var length = Math.Round(Math.Log10(input)/Math.Log10(5), MidpointRounding.ToZero);
        var rest = input;
        double current = 0;
        List<int> result = new List<int>();
        while (length >= 0)
        {
            var divider = Math.Pow(5, length);
            current = Math.Round(rest / divider, MidpointRounding.ToZero);
            rest = rest % divider;
            result.Add((int)current);
            length--;
        }

        // We will start at the back and write away the shizzle
        for (int i = result.Count - 1; i >= 0; i--)
        {
            if (result[i] >= 3)
            {
                result[i] -= 5;
                if (i == 0)
                {
                    result.Insert(0, 1);
                }
                else
                {
                    result[i-1] += 1;
                }
            }
        }

        return new String(result.Select(i => ConvertCharBack(i)).ToArray());
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day25.txt");
        var allNumbers = lines
            .Select(l=>ParseLine(l))
            .ToArray();

        var sum = allNumbers.Aggregate((a,b)=>a+b); // This is how you do it with BigInteger
        System.Console.WriteLine($"part1: {ToSnafu(sum)}");
    }

}
