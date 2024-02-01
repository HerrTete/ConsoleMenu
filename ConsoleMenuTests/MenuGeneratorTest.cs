using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleTools;
using Xunit;

namespace ConsoleMenuTests
{
  public class MenuGeneratorTest
  {
    [Fact]
    public void CollectEntryPoints_Test()
    {
      var foundEntryPoints = MenuGenerator.CollectEntryPoints(new[] { Assembly.GetExecutingAssembly() });
      Assert.Equal(1,foundEntryPoints.Count);
      Assert.Equal("Do stuff",foundEntryPoints[0].Name);
      Assert.Equal("/SubMenu",foundEntryPoints[0].Path);
    }

    [EntryPoint(name:"Do stuff", path:"SubMenu")]
    public static void TestDoStuff() { }

    [Fact]
    public void GenerateMenu_Test()
    {
      var entryPoints = new List<EntryPointAttribute>();
      entryPoints.Add(new EntryPointAttribute("Schlupp", "Hans/Bärbel/Isolde"));
      entryPoints.Add(new EntryPointAttribute("Peter", "Hans"));
      entryPoints.Add(new EntryPointAttribute("Dieter", "Hans"));
      entryPoints.Add(new EntryPointAttribute("Willi", "Hans/Franz"));
      entryPoints.Add(new EntryPointAttribute("Butz", "Hans/Schorsch/Fritz"));
      entryPoints.Add(new EntryPointAttribute("Bernd"));
      var menu = MenuGenerator.GenerateMenu(entryPoints);
      Assert.Equal(2,menu.Items.Count);
      Assert.Contains("Hans", menu.Items.Select(i => i.Name));
      Assert.Contains("Bernd", menu.Items.Select(i => i.Name));
    }
  }
}
