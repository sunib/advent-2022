namespace advent_of_code;



public class SimpleNode {
    public SimpleNode(string name, SimpleNode parent, int size = 0)
    {
        Name = name;
        Size = size;
        Parent = parent;
    }

    public int Size { get; set; }   // This is the total size of all children!
    public string Name { get; set; }
    public List<SimpleNode> Children {get; set; } = new List<SimpleNode>();
    public SimpleNode? Parent {get; set;}
}

public class Day7
{
    public async Task Execute()
    {
        var root = new SimpleNode("root", null, 0);
        var currentNode = root;
        var lines = await File.ReadAllLinesAsync("day7_5mb_large.txt");
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
                    currentNode.Children.Add(new SimpleNode(parts[1], currentNode, 0));
                }
                else
                {
                    var fileSize = int.Parse(parts[0]);                    
                    // Move up and add number of bytes so that we have our totals
                    var sumNode = currentNode;
                    while (sumNode != null)
                    {
                        sumNode.Size += fileSize;
                        sumNode = sumNode.Parent;
                    }
                }
            }
        }

        // Now we will need to recursive to count to amount of folders
        var totalSize = 70000000;
        var required = 30000000;
        var currentlyFree = totalSize - root.Size;
        if (currentlyFree < required) 
        {
            var toBeDeletedFolder = CountBigFolders(root, root, required - currentlyFree);
            Console.WriteLine("The end: " + toBeDeletedFolder.Size);
        }
        else
        {
            Console.Write("Already enough space free");
        }        
    }

    public SimpleNode CountBigFolders(SimpleNode start, SimpleNode currentLowest, int minimalDeleted)
    {
        SimpleNode result = currentLowest;        
        if (start.Size <= currentLowest.Size && start.Size >= minimalDeleted && start.Parent != null)
        {
            result = start;
        }

        foreach (var folder in start.Children)
        {
            result = CountBigFolders(folder, result, minimalDeleted);
        }
        
        return result;
    }
}
