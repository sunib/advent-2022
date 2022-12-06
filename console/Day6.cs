using System.Text.RegularExpressions;

namespace advent_of_code;



public class Day6
{
    public int FindPosition(string input, int uniqueChars)
    {
        for (int i = uniqueChars; i < input.Length; i++)
        {
            if (input.Skip(i-uniqueChars).Take(uniqueChars).Distinct().Count() == uniqueChars)
                return i;   
        }

        return 0;
    }

    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day6.txt");
        // That would be one line :-) with 4096 chars

        // Walk through the line and keep a buffer that allows you to see if all is different or not
        // That could just be a simple if if you want to make it fast.

        string input = "mjqjpqmgbljsphdztnvjfqwrcgsmlb";
                      //012345678901234567890
                      //       ^
        //var position = FindPosition(input,14);
        var position = FindPosition(lines[0],14);
        System.Console.WriteLine(position);
    }
}