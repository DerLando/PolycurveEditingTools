using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class PolycurveEditGrips : CustomObjectGrips
    {
        private PolycurveEditGrip[] _editgrips;
        private Plane _plane;
        private Curve[] _activePolycurve;
        private Curve[] _originalPolycurve;
        private bool _drawPolycurve;

        public PolycurveEditGrips()
        {
            _drawPolycurve = false;
        }

        public bool CreateGrips(PolyCurve polyCurve)
        {
            // if grips are already created, return
            if (GripCount > 0) return false;

            // generate active and original curve arrays from input
            _originalPolycurve = polyCurve.DuplicateSegments();
            _activePolycurve = new Curve[polyCurve.SegmentCount];
            Array.Copy(_originalPolycurve, _activePolycurve, polyCurve.SegmentCount);

            // List to hold all editgrips
            var grips = new List<PolycurveEditGrip>();

            var tol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            foreach (var curve in _activePolycurve)
            {
                // get locations of all grips
                var locations = (from grip in grips select grip.OriginalLocation).ToArray();

                // start and end points of curves are always edit points
                if (!locations.Any(l => l.EpsilonEquals(curve.PointAtStart, tol)))
                    grips.Add(new PolycurveEditGrip(curve.PointAtStart));
                if (!locations.Any(l => l.EpsilonEquals(curve.PointAtEnd, tol)))
                    grips.Add(new PolycurveEditGrip(curve.PointAtEnd));

                switch (curve.CurveType())
                {
                    case CurveType.LineCurve:
                        // lines can be edited from only start and end, do nothing
                        break;
                    case CurveType.ArcCurve:
                        // arcs are a bit more complicated :(
                        break;
                    case CurveType.NurbsCurve:
                        // convert to nurbs
                        var nurbs = curve.ToNurbsCurve();
                        for (int i = 1; i < nurbs.Points.Count - 1; i++) // first and last controlpoints are `PointAtStart` and `PointAtEnd` -> already in list
                        {
                            grips.Add(new PolycurveEditGrip(nurbs.Points[i].Location));
                        }
                        break;
                    case CurveType.Undefined:
                        throw new ArgumentException($"{curve} could not be determined as either arc, line or nurbs!");
                        break;
                }
            }

            // set grips array from grips list
            _editgrips = grips.ToArray();
            return true;
        }
    }
}
