namespace advent_of_code;

public enum Handsign {
    Rock = 1,    // Rock defeats scissors
    Paper = 2,   // Paper defeats rock
    Scissors = 3 // Scissors defeats paper
}

public enum DesiredOutcome
{
    Lose = 1,
    Draw = 2,
    Win = 3
}

public record Instruction(Handsign input, DesiredOutcome outcome);
public record Round(Handsign Them, Handsign Us);


public static class DayTwo 
{
    public static Round DefineRequiredAction(Instruction instruction) 
    {
        int result = (int)instruction.input;
        if (instruction.outcome == DesiredOutcome.Draw)
        {
            result = (int)instruction.input;
        }
        else if (instruction.outcome == DesiredOutcome.Lose)
        {
            result = (int)instruction.input - 1;
            if (result == 0) {
                result = 3;
            }
        }
        else if (instruction.outcome == DesiredOutcome.Win) 
        {
            result = (int)instruction.input + 1;
            if (result == 4) {
                result = 1;
            }
        }

        return new Round(instruction.input, (Handsign)result);
    }

    public static int CalculateScore(Round round) {
        int result = (int)round.Us;
        if (round.Them == round.Us) {
            // It's a draw
            result += 3;
        }        
        else if (round.Us - round.Them == 1 || round.Us == Handsign.Rock && round.Them == Handsign.Scissors)
        {
            // We win
            result += 6;
        }
        else
        {
            // They win
            result += 0;
        }

        return result;
    }

    public static async Task Run()
    {
        var allLines = await File.ReadAllLinesAsync("day2.txt");
        var rounds = allLines
            .Select(s => new Instruction(
                (Handsign)(int)s[0]-(int)'A' + 1, 
                (DesiredOutcome)(int)s[2]-(int)'X' + 1))
            .Select(i => DefineRequiredAction(i))
            .ToList();
        
        var score = rounds.Sum(r => CalculateScore(r));
        System.Console.WriteLine(score);
    }
}