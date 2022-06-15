using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Squisher
{
    public class SquishBackComponent : GH_Component
    {
        public SquishBackComponent() : base("Squish Back", "Squish Back", "Maps 2D geometry from a squished mesh or surface back to 3D.", "Transform", "Squisher") { }
        public override Guid ComponentGuid => new Guid("3cd5a318-c9da-425a-868e-e5634f7f5798");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SquishBackIcon;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Squished", "S", "Squished Geometry to map back to.", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometry", "G", "2D geometry to map back.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Geometry mapped back to 3D.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic inputSquishedGeometry = 0;
            List<dynamic> geometriesToMap = new List<dynamic>();

            DA.GetData(0, ref inputSquishedGeometry);
            DA.GetDataList(1, geometriesToMap);

            Mesh inputMesh;
            Surface inputSurface;

            List<dynamic> outputMappedGeometries = new List<dynamic>();

            if (!Rhino.Geometry.Squisher.Is2dPatternSquished(inputSquishedGeometry.Value))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input geometry must be a a result of a squish operation.");
                return;
            }

            List<GeometryBase> geometries = new List<GeometryBase>();
            foreach (dynamic geo in geometriesToMap)
            {
                // For some reason, Point3d deoesn't fit within the type GeometryBase, so we create an intermediate "Point" type.
                if (geo.Value is Point3d)
                {
                    Point pt = new Point(geo.Value);
                    geometries.Add(pt);
                }
                else
                {
                    // just in case of a weird unexpected input
                    try { geometries.Add(geo.Value); }
                    catch { }
                }
            }
            

            if (inputSquishedGeometry.Value is Mesh)
            {
                inputMesh = inputSquishedGeometry.Value;
                outputMappedGeometries.AddRange(Rhino.Geometry.Squisher.SquishBack2dMarks(inputMesh, geometries));
            }
            else if (inputSquishedGeometry.Value is Brep)
            {
                Brep untrimmedInput = inputSquishedGeometry.Value;
                inputSurface = untrimmedInput.Faces[0].UnderlyingSurface();
                outputMappedGeometries.AddRange(Rhino.Geometry.Squisher.SquishBack2dMarks(inputSurface, geometries));
            }


            DA.SetDataList(0, outputMappedGeometries);
        }
    }
}