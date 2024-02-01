using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleTools;

internal class MenuGenerator
{
  internal static ConsoleMenu Generate()
  {
    return GenerateMenu(CollectEntryPoints(), string.Empty);
  }
  private static ConsoleMenu GenerateMenu(List<EntryPointAttribute> entryPoints, string currentPath)
  {
    var menu = new ConsoleMenu();
    foreach (var entryPoint in entryPoints.Where(e => e.Path == currentPath))
    {
      menu.Add(entryPoint.Name, entryPoint.Action);
    }

    var subMenus = entryPoints.Where(e => e.Path.StartsWith(currentPath)).Select(e => e.Path.Substring(currentPath.Length)).Distinct().Where(s => !string.IsNullOrEmpty(s)).Order();

    foreach (var subMenuPath in subMenus.Where(s =>  s.Length > 1 && !s.Substring(1).Contains('/')))
    {
      var split = subMenuPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
      var newPath = split[0];
      var subMenu = GenerateMenu(entryPoints, currentPath + '/' + newPath);
      menu.Add(subMenuPath.Substring(1), subMenu.Show);
    }

    menu.Add("BACK", ConsoleMenu.Close);
    return menu;
  }

  private static List<EntryPointAttribute> CollectEntryPoints()
  {
    var entryPoints = new List<EntryPointAttribute>();
    var types = Assembly.GetExecutingAssembly().GetTypes();
    foreach (var type in types)
    {
      var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
      foreach (var methodInfo in methods)
      {
        var attribute = methodInfo.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(EntryPointAttribute));
        if (attribute is not null && attribute is EntryPointAttribute entryPoint)
        {
          entryPoint.Action = () => { methodInfo.Invoke(null, null); };
          entryPoints.Add(entryPoint);
        }
      }
    }

    entryPoints.Add(new EntryPointAttribute("Schlupp", "Hans/Bärbel/Isolde"));
    entryPoints.Add(new EntryPointAttribute("Peter", "Hans"));
    entryPoints.Add(new EntryPointAttribute("Dieter", "Hans"));
    entryPoints.Add(new EntryPointAttribute("Willi", "Hans/Franz"));
    entryPoints.Add(new EntryPointAttribute("Butz", "Hans/Schorsch/Fritz"));
    entryPoints.Add(new EntryPointAttribute("Bernd"));

    return entryPoints;
  }
}
