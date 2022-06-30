using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Rhino.Geometry.Collections;

namespace Squisher
{
    public class SquisherComponent : GH_Component
    {
        public SquisherComponent() : base("Squisher", "Squisher", "It flattens a non-developable 3-D mesh or NURBS surface into a flat 2-D pattern.", "Transform", "Squisher") { }
        public override Guid ComponentGuid => new Guid("D154157F-F77C-4039-B39A-5C951D0A5CB3");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SquishIcon;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to squish. Can be a mesh or surface.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Parameters", "P", "Squishing parameters. If empty, default parameters will be used.", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Map", "M", "Geometry to map to the flattened geometry. Can be a point or curve.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[2].DataMapping = GH_DataMapping.Flatten;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Squished", "S", "Squished geometry.", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Mapped", "M", "Mapped geometry on the squished geometry.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic inputGeometry = 0;
            SquishParameters parameters = null;
            List<dynamic> geometriesToMap = new List<dynamic>();

            DA.GetData(0, ref inputGeometry);
            DA.GetData(1, ref parameters);
            DA.GetDataList(2, geometriesToMap);

            Mesh inputMesh;
            Surface inputSurface;

            Mesh outputMesh;
            Brep outputSurface;
            List<dynamic> outputMappedGeometries = new List<dynamic>();

            if (inputGeometry.Value is Mesh)
            {
                inputMesh = inputGeometry.Value;
                
                // We check if the provided input is valid
                if (!inputMesh.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input mesh must be valid!");
                    return;
                }

                // If no parameters are provided, we use the default one
                if (parameters == null)
                    parameters = SquishParameters.Default;

                // No geometries to map, we can squish and output the results
                if (geometriesToMap.Count == 0)
                {
                    Rhino.Geometry.Squisher squisher = new Rhino.Geometry.Squisher();
                    outputMesh = squisher.SquishMesh(parameters, inputMesh);
                    DA.SetData(0, outputMesh);
                }
                // we have things to map:
                else
                {
                    Rhino.Geometry.Squisher squisher = new Rhino.Geometry.Squisher();
                    outputMesh = squisher.SquishMesh(parameters, inputMesh);
                    DA.SetData(0, outputMesh);
                    // Point3d and Curve geometries are supported
                    foreach (dynamic element in geometriesToMap)
                    {
                        if (element == null)
                            continue;

                        if (element.Value is Point3d)
                        {
                            Point3d  resultPt;
                            squisher.SquishPoint((Point3d)element.Value, out resultPt);
                            outputMappedGeometries.Add(resultPt);
                        }
                        if (element.Value is Curve)
                        {
                            PolylineCurve resultCrv = squisher.SquishCurve((Curve)element.Value);
                            outputMappedGeometries.Add(resultCrv);
                        }
                    }
                    DA.SetDataList(1, outputMappedGeometries);
                }
            }


            else if (inputGeometry.Value is Brep)
            {
                Brep untrimmedInput = inputGeometry.Value;

                BrepFace bb = untrimmedInput.Faces[0];

                // We extract the untrimmed surface
                inputSurface = bb.UnderlyingSurface();

                // If no parameters are provided, we use the default one
                if (parameters == null)
                    parameters = SquishParameters.Default;

                // No geometries to map, we can squish and output the results
                if (geometriesToMap.Count == 0)
                {
                    Rhino.Geometry.Squisher squisher = new Rhino.Geometry.Squisher();
                    outputSurface = squisher.SquishSurface(parameters, inputSurface);
                    DA.SetData(0, outputSurface);
                }
                else
                {
                    Rhino.Geometry.Squisher squisher = new Rhino.Geometry.Squisher();
                    outputSurface = squisher.SquishSurface(parameters, inputSurface);
                    DA.SetData(0, outputSurface);
                    // Point3d and Curve geometries are supported
                    foreach (dynamic element in geometriesToMap)
                    {
                        if (element == null)
                            continue;

                        if (element.Value is Point3d)
                        {
                            Point3d resultPt;
                            squisher.SquishPoint((Point3d)element.Value, out resultPt);
                            outputMappedGeometries.Add(resultPt);
                        }
                        if (element.Value is Curve)
                        {
                            PolylineCurve resultCrv = squisher.SquishCurve((Curve)element.Value);
                            outputMappedGeometries.Add(resultCrv);
                        }
                    }
                    DA.SetDataList(1, outputMappedGeometries);
                }
            }
            
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input geometry must be a mesh or a surface.");
                return;
            }
        }
    }
}