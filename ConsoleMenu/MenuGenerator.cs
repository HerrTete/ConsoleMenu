using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConsoleTools;

public class MenuGenerator
{
  public static ConsoleMenu Generate(IEnumerable<Assembly> assemblies)
  {
    return GenerateMenu(CollectEntryPoints(assemblies));
  }

  internal static ConsoleMenu GenerateMenu(List<EntryPointAttribute> entryPoints, string currentPath = "/",
    string? backMenuName = "BACK")
  {
    var menu = new ConsoleMenu();
    foreach (var entryPoint in entryPoints.Where(e => e.Path == currentPath))
    {
      menu.Add(entryPoint.Name, entryPoint.Action);
    }

    var subMenus = entryPoints.Where(e => e.Path.StartsWith(currentPath))
      .Select(e => e.Path.Substring(currentPath.Length)).Distinct().Where(s => !string.IsNullOrEmpty(s))
      .OrderBy(s => s);

    foreach (var subMenuPath in subMenus.Where(s => s.Length > 1 && !s.Substring(1).Contains('/')))
    {
      var split = subMenuPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
      var newPath = split[0];
      var subMenu = GenerateMenu(entryPoints, currentPath + '/' + newPath);
      menu.Add(subMenuPath, subMenu.Show);
    }

    if (backMenuName is null)
    {
      menu.Add(backMenuName, ConsoleTools.ConsoleMenu.Close);
    }

    return menu;
  }

  internal static List<EntryPointAttribute> CollectEntryPoints(IEnumerable<Assembly> assemblies)
  {
    var entryPoints = new List<EntryPointAttribute>();
    foreach (var assembly in assemblies)
    {
      var types = assembly.GetTypes();
      foreach (var type in types)
      {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var methodInfo in methods)
        {
          var attribute = methodInfo.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType() == typeof(EntryPointAttribute));
          if (attribute is not null && attribute is EntryPointAttribute entryPoint)
          {
            entryPoint.Action = () => { methodInfo.Invoke(null, null); };
            entryPoints.Add(entryPoint);
          }
        }
      }
    }

    return entryPoints;
  }
}
