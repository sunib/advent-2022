namespace advent_of_code_day10;


public class Display 
{
    public int Row { get; set; } = 0;
    public int Col { get; set; } = 0;

    public bool[,] display = new bool[40,6];

    public void DrawPixel(int spriteLocation)
    {
        // This dependes on the sprit location
        display[Col,Row] = Col >= (spriteLocation - 1) && Col <= (spriteLocation + 1);

        // Jump to next location
        Col++;
        if (Col >= 40)
        {
            Col = 0;
            Row++;
            if (Row >= 6)
            {
                Row = 0;
            }
        }
    }

    public void PrintDisplay() {
        for (int y = 0; y < display.GetLength(1); y++)
        {
            for (int x = 0; x < display.GetLength(0); x++)
            {
                Console.Write(display[x,y] ? "#" : ".");
            }
            
            Console.Write(System.Environment.NewLine);       
        }
    }
}


public class Cpu
{
    public int pc { get; set; } = 0;
    public int regx { get; set; } = 1;
    public int answer1 {get;set;} = 0;
    public int offset {get;set;} = 0;
    public int period { get; set; } = 20;
    public Display display = new Display();

    public void Clock(int count = 1)
    {
        while (--count >= 0)
        {
            pc++;
            display.DrawPixel(regx);
            if ((pc + offset) % period == 0)
            {
                period = 40;
                offset = 20;
                answer1 += pc * regx;
            }
        }
    }    
}

public class Day10
{
    public async Task Execute()
    {
        var lines = await File.ReadAllLinesAsync("console/day10.txt");
        
        var cpu = new Cpu();
        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            switch (parts[0])
            {
                case "noop":
                    cpu.Clock();
                    break;
                case "addx":
                    cpu.Clock(2);
                    cpu.regx += int.Parse(parts[1]);
                    break;
                default:
                    throw new InvalidDataException("Unknown action");
            }


        }

        System.Console.WriteLine("part1: " + cpu.answer1);    
        System.Console.WriteLine("part2: ");
        cpu.display.PrintDisplay();
    }
}
