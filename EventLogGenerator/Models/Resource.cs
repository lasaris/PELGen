namespace EventLogGenerator.Models;

// Resource that can be used with combination of Activity
public class Resource
{
    // Name of resource. Should be unique
    public string Name;

    public Resource(string name)
    {
        Name = name;
    }
}