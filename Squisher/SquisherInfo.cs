using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Squisher
{
    public class SquisherInfo : GH_AssemblyInfo
    {
        public override string Name => "Squisher";
        public override Bitmap Icon => null;
        public override string Description => "Squisher is a plugin that wrap the Squish functions from Rhino into Grasshopper.";
        public override Guid Id => new Guid("1166515F-75C4-40C1-AE0D-8A4F94F469B9");
        public override string AuthorName => "Ayoub Lharchi";
        public override string AuthorContact => "alha@kglakademi.dk";
    }
}