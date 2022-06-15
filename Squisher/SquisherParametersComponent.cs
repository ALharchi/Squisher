using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Squisher
{
    public class SquisherParametersComponent : GH_Component
    {
        public SquisherParametersComponent() : base("Squisher Parameters", "Squisher Parameters", "Parameters used to squish geometry. You can read more about these parameters here: https://developer.rhino3d.com/api/RhinoCommon/html/T_Rhino_Geometry_SquishParameters.htm", "Transform", "Squisher") { }
        public override Guid ComponentGuid => new Guid("d2a8fd02-7959-4eaa-b614-71d497f4c93a");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SquishParametersIcon;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Absolute Limit", "AL", " If -1.0 <= AbsoluteLimit < 0.0, then then an absolute compression limit is applied so that (2d length)/(3d length) >= abs(AbsoluteLimit). In particular, Absolute = -1.0, then no compression is permitted(2d length) >= (3d length). If 0.0 < m_absolute_limit <= 1.0 then then an absolute stretching limit is applied so that (2d length)/(3d length) <= 1/abs(AbsoluteLimit). Examples: AbsoluteLimit 1.0: no stretching, (2d length) <= 1.0*(3d length) 0.5: cap on stretching, 0.5*(2d length) <= (3d length) -0.5: cap on compression, (2d length) >= 0.5*(3d length) -1.0: no compression, (2d length) >= 1.0*(3d length)", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Algorithm", "A", "Flattening algorithm to be used. True for Geometric, False for Physical Stress.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Boundary Compress Constant", "BCC", "Spring constant for compressed boundary edges times the rest length", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Boundary Stretch Constant", "BSC", "Spring constant for stretched boundary edges", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Interior Compress Constant", "ICC", "Spring constant for compressed interior edges times the rest length", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Interior Stretch Constant", "ISC", "Spring constant for stretched boundary edges times the rest length", GH_ParamAccess.item, 1.0);
            pManager.AddBooleanParameter("Preserve Topology", "PT", "The mesh has coincident vertices and PreserveTopology is true, then the flattening is based on the mesh's topology and coincident vertices will remain coincident. Otherwise coincident vertices are free to move apart.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Save Mapping", "SM", "", GH_ParamAccess.item, true); // Do I need this?
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Parameters", "P", "Squishing Parameters", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double absoluteLimit = 0;
            bool algorithm = false;
            double boundaryCompressConstant = 0;
            double boundaryStretchConstant = 0;
            double interiorCompressConstant = 0;
            double interiorStretchConstant = 0;
            bool preserveTopology = false;
            bool saveMapping = false;

            DA.GetData(0, ref absoluteLimit);
            DA.GetData(1, ref algorithm);
            DA.GetData(2, ref boundaryCompressConstant);
            DA.GetData(3, ref boundaryStretchConstant);
            DA.GetData(4, ref interiorCompressConstant);
            DA.GetData(5, ref interiorStretchConstant);
            DA.GetData(6, ref preserveTopology);
            DA.GetData(7, ref saveMapping);

            SquishParameters parameters = new SquishParameters();
            parameters.AbsoluteLimit = absoluteLimit;
            if (algorithm)
                parameters.Algorithm = SquishFlatteningAlgorithm.Geometric;
            else
                parameters.Algorithm = SquishFlatteningAlgorithm.PhysicalStress;
            parameters.BoundaryCompressConstant = boundaryCompressConstant;
            parameters.BoundaryStretchConstant = boundaryStretchConstant;
            parameters.InteriorCompressConstant = boundaryCompressConstant;
            parameters.InteriorStretchConstant = boundaryStretchConstant;
            parameters.PreserveTopology = preserveTopology;
            parameters.SaveMapping = saveMapping;


            DA.SetData(0, parameters);
        }

    }
}