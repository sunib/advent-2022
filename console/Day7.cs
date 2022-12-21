namespace advent_of_code;
using System.Threading;




public class SimpleNode {
    public SimpleNode(string name, SimpleNode parent)
    {
        Name = name;
        Size = 0;
        Parent = parent;
        FullName = $"{parent?.FullName ?? ""}/{name}";
        
        
        
    }

    public string FullName { get;set; }

    public int Size { get; set; }   // This is the total size of all children!
    public string Name { get; set; }
    public List<SimpleNode> Children {get; set; } = new List<SimpleNode>();
    public SimpleNode? Parent {get; set;}
}

public class Day7
{
    public async Task Execute()
    {
        var root = new SimpleNode("root", null);
        var root2 = new Dictionary<string, int>();
        var currentNode = root;
        var lines = await File.ReadAllLinesAsync("console/day7_5mb_large.txt");
        int lineCount = 0;
        foreach (var line in lines)
        {
            if (lineCount++ % 10000 == 0)
            {
                System.Console.WriteLine(((float)lineCount / (float)lines.Length) * 100 + "%");
            }

            var parts = line.Split(' ');
            if (parts[0] == "$")
            {
                if (parts[1] == "cd")
                {
                    if (parts[2] == "..")
                    {
                        if (currentNode.Parent != null)
                        {
                            currentNode = currentNode.Parent;
                        }
                    }
                    else if (parts[2] == "/")
                    {
                        currentNode = root;
                    }
                    else
                    {
                        // Check if it's there, otherwise create?
                        currentNode = currentNode.Children.Find(sn => sn.Name == parts[2]);
                    }
                }
            }
            else 
            {
                if (parts[0] == "dir")
                {
                    var newNode = new SimpleNode(parts[1], currentNode);
                    currentNode.Children.Add(newNode);
                    root2.Add(newNode.FullName, 0);
                }
                else
                {
                    var fileSize = int.Parse(parts[0]);                    
                    currentNode.Size += fileSize;
                    // Move up and add number of bytes so that we have our totals
                    // var sumNode = currentNode;
                    // while (sumNode != null)
                    // {
                    //     
                    //     sumNode = sumNode.Parent;
                    // }
                }
            }
        }

        // Now we will need to recursive to count to amount of folders
        var totalSize = 70000000;
        var required = 30000000;
        System.Console.WriteLine("Start calculate on file sizes");
        CalculateSizes(root);    // First traverse so that we know how big it is
        System.Console.WriteLine("Total size = " + root.Size);
        var currentlyFree = totalSize - root.Size;
        if (currentlyFree < required) 
        {
            System.Console.WriteLine("Now starting to find the best fit");
            var toBeDeletedFolder = CountBigFolders(root, root, required - currentlyFree);
            Console.WriteLine("The end: " + toBeDeletedFolder.Size);
        }
        else
        {
            Console.Write("Already enough space free");
        }        
    }

    public void CalculateSizes(SimpleNode current)
    {
        foreach (var folder in current.Children)
        {
            CalculateSizes(folder);
            current.Size += folder.Size;
        }
    }

    public SimpleNode CountBigFolders(SimpleNode start, SimpleNode currentLowest, int minimalSize = 0)
    {
        SimpleNode result = currentLowest;
        if (start.Size <= currentLowest.Size && start.Size >= minimalSize && start.Parent != null)
        {
            result = start;
        }

        foreach (var folder in start.Children)
        {
            result = CountBigFolders(folder, result, minimalSize);
        }

        return result;
    }
}
