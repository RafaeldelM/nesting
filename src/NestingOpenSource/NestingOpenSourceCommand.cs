using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NestingLibPort;
using NestingLibPort.Data;
using NestingLibPort.Util;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            NestPath bin = new NestPath();
            double binWidth = 1000;
            double binHeight = 1000;
            bin.add(0, 0);
            bin.add(binWidth, 0);
            bin.add(binWidth, binHeight);
            bin.add(0, binHeight);
            Console.WriteLine("Bin Size : Width = " + binWidth + " Height=" + binHeight);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var testXML = Path.Combine(path, "test.xml");
            var nestPaths = SvgUtil.transferSvgIntoPolygons(testXML);
            Console.WriteLine("Reading File = test.xml");
            Console.WriteLine("No of parts = " + nestPaths.Count);
            Config config = new Config();
            Console.WriteLine("Configuring Nest");
            Nest nest = new Nest(bin, nestPaths, config, 2);
            Console.WriteLine("Performing Nest");
            List<List<Placement>> appliedPlacement = nest.startNest();
            Console.WriteLine("Nesting Completed");
            var svgPolygons = SvgUtil.svgGenerator(nestPaths, appliedPlacement, binWidth, binHeight);
            Console.WriteLine("Converted to SVG format");
            SvgUtil.saveSvgFile(svgPolygons, "output.svg");
            Console.WriteLine("Saved svg file..Opening File");
            Process.Start("output.svg");
            Console.ReadLine();

            return Result.Success;
        }
    }
}
