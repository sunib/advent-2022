namespace advent_of_code;



public class SimpleNode {
    public SimpleNode(string name, bool isDir, SimpleNode parent, int size = 0)
    {
        Name = name;
        Size = size;
        Parent = parent;
        IsDir = isDir;
    }

    public int Size { get; set; }   // This is the total size of all children!
    public string Name { get; set; }
    public bool IsDir { get; set; }
    public List<SimpleNode> Children {get; set; } = new List<SimpleNode>();
    public SimpleNode? Parent {get; set;}
}

public class Day7
{
    public async Task Execute()
    {
        var root = new SimpleNode("root", true, null, 0);
        var currentNode = root;
        var lines = await File.ReadAllLinesAsync("console/day7.txt");
        foreach (var line in lines)
        {
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
                    currentNode.Children.Add(new SimpleNode(parts[1], true, currentNode, 0));
                }
                else
                {
                    var fileSize = int.Parse(parts[0]);
                    currentNode.Children.Add(new SimpleNode(parts[1], false, currentNode, fileSize));
                    
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
        if (start.IsDir && start.Size <= currentLowest.Size && start.Size >= minimalDeleted && start.Parent != null)
        {
            result = start;
        }

        foreach (var folder in start.Children.Where(i => i.IsDir))
        {
            result = CountBigFolders(folder, result, minimalDeleted);
        }
        
        return result;
    }
}
