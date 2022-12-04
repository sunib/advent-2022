namespace advent_of_code;

public static class DayOne
{
    public static async Task Execute()
    {
        var allLines = await File.ReadAllLinesAsync("day1.txt");

        int currentCalorie = 0;
        var elfCalories = new List<int>();
        foreach (var line in allLines)
        {
            if (int.TryParse(line, out var value))
            {
                currentCalorie += value;
            }
            else 
            {
                elfCalories.Add(currentCalorie);        
                currentCalorie = 0;
            }
        }

        var highestThree = elfCalories.OrderDescending().Take(3).Sum();
        System.Console.WriteLine(highestThree);

    }
}