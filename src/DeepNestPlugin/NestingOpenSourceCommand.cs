using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using DeepNestLib;
using Eto.Forms;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Command = Rhino.Commands.Command;
using Result = Rhino.Commands.Result;

namespace NestingOpenSource
{
    public class NestingOpenSourceCommand : Command
    {
        public NestingOpenSourceCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static NestingOpenSourceCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "Nesting"; }
        }

        NestingContext context = new NestingContext();
        Thread th;

        List<NFP> sheets { get { return context.Sheets; } }
        List<NFP> polygons { get { return context.Polygons; } }
        public SvgNest nest { get { return context.Nest; } }

        bool stop = false;
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

          //  var cnt = GetCountFromDialog();
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                var xx = r.Next(2000) + 100;
                var yy = r.Next(2000);
                var ww = r.Next(60) + 10;
                var hh = r.Next(60) + 5;
                NFP pl = new NFP();
                int src = 0;
                if (polygons.Any())
                {
                    src = polygons.Max(z => z.source.Value) + 1;
                }
                polygons.Add(pl);
                pl.source = src;
                pl.x = xx;
                pl.y = yy;
                pl.Points = new SvgPoint[] { };
                pl.AddPoint(new SvgPoint(0, 0));
                pl.AddPoint(new SvgPoint(ww, 0));
                pl.AddPoint(new SvgPoint(ww, hh));
                pl.AddPoint(new SvgPoint(0, hh));
            }
            // UpdateList();


            List<Sheet> sh = new List<Sheet>();
            var srcAA = context.GetNextSheetSource();

            sh.Add(NewSheet(3000, 2000));
            foreach (var item in sh)
            {
                item.source = srcAA;
                context.Sheets.Add(item);
            }


            if (sheets.Count == 0 || polygons.Count == 0)
            {
                MessageBox.Show("There are no sheets or parts",  MessageBoxButtons.OK);
                return Result.Success; ;
            }
            stop = false;
          //  progressBar1.Value = 0;
          //  tabControl1.SelectedTab = tabPage4;
            context.ReorderSheets();
            RunDeepnest();

            return Result.Success;
        }

        public Sheet NewSheet(int w = 3000, int h = 1500)
        {
            var tt = new RectangleSheet();
            tt.Name = "rectSheet" + (sheets.Count + 1);
            tt.Height = h;
            tt.Width = w;
            tt.Rebuild();

            return tt;
        }

        public void RunDeepnest()
        {

            if (th == null)
            {
                th = new Thread(() =>
                {
                    context.StartNest();
                    UpdateNestsList();
                    Background.displayProgress = displayProgress;

                    while (true)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        context.NestIterate();
                        UpdateNestsList();
                        displayProgress(1.0f);
                        sw.Stop();
                        //toolStripStatusLabel1.Text = "Nesting time: " + sw.ElapsedMilliseconds + "ms";
                        if (stop) break;
                    }
                    th = null;
                });
                th.IsBackground = true;
                th.Start();
            }
        }

        internal void displayProgress(float progress)
        {
            RhinoApp.WriteLine(progress.ToString());
            progressVal = progress;

        }
        public float progressVal = 0;
        public void UpdateNestsList()
        {
            //if (nest != null)
            //{
            //    listView4.Invoke((Action)(() =>
            //    {
            //        listView4.BeginUpdate();
            //        listView4.Items.Clear();
            //        foreach (var item in nest.nests)
            //        {
            //            listView4.Items.Add(new ListViewItem(new string[] { item.fitness + "" }) { Tag = item });
            //        }
            //        listView4.EndUpdate();
            //    }));
            //}
        }




    }
}
