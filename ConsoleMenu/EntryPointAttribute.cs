using System;

namespace ConsoleTools;

[AttributeUsage(AttributeTargets.Method)]
public class EntryPointAttribute : Attribute
{
  public string Path { get; private set; }
  public string Name { get; private set; }
  public Action Action { get; set; } = () => { };

  public EntryPointAttribute(string name, string path = "/")
  {
    if (!path.StartsWith("/"))
    {
      path = "/" + path;
    }

    Path = path;
    Name = name;
  }
}
