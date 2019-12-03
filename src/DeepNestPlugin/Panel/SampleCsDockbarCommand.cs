using Rhino;
using Rhino.Commands;

namespace NestingOpenSource.Panel
{
  public class SampleCsDockbarCommand : Command
  {
    public override string EnglishName => "Deepnest";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      RhinoWindows.Controls.DockBar.Show(WpfDockBar.BarId, false);
      return Result.Success;
    }
  }
}
