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
        private int[] _curveSegmentGripCount;
        private bool _drawPolycurve;
        private bool _polycurveIsClosed;

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
            _activePolycurve = new Curve[_originalPolycurve.Length];
            Array.Copy(_originalPolycurve, _activePolycurve, _originalPolycurve.Length);
            _polycurveIsClosed = polyCurve.IsClosed;

            // List to hold all editgrips
            var grips = new List<PolycurveEditGrip>();
            _curveSegmentGripCount = new int[_originalPolycurve.Length];

            var tol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            for (var i = 0; i < _activePolycurve.Length; i++)
            {
                var curve = _activePolycurve[i];

                grips.Add(new PolycurveEditGrip(curve.PointAtStart));

                switch (curve.CurveType())
                {
                    case CurveType.LineCurve:
                        // lines can be edited from only start and end, do nothing
                        _curveSegmentGripCount[i] = 1;
                        break;
                    case CurveType.ArcCurve:
                        // arcs are a bit more complicated :(
                        grips.Add(new PolycurveEditGrip(curve.PointAtNormalizedLength(0.5)));
                        _curveSegmentGripCount[i] = 2;
                        break;
                    case CurveType.NurbsCurve:
                        // convert to nurbs
                        var nurbs = curve.ToNurbsCurve();
                        for (int j = 0; j < nurbs.Points.Count - 1; j++) // first and last controlpoints are `PointAtStart` and `PointAtEnd` -> already in list
                        {
                            grips.Add(new PolycurveEditGrip(nurbs.Points[j].Location));
                        }

                        _curveSegmentGripCount[i] = nurbs.Points.Count - 1;
                        break;
                    case CurveType.Undefined:
                        throw new ArgumentException($"{curve} could not be determined as either arc, line or nurbs!");
                        break;
                }
            }

            // on open curves, add end point of last segment
            if(!_polycurveIsClosed) grips.Add(new PolycurveEditGrip(_activePolycurve[_activePolycurve.Length - 1].PointAtEnd));

            // set grips array from grips list
            _editgrips = grips.ToArray();

            // use rhino native function to actually add grips
            foreach (var polycurveEditGrip in _editgrips)
            {
                AddGrip(polycurveEditGrip);
            }

            return true;
        }

        private int[] GetCornerGripIndices()
        {
            int cornerIndex = 0;
            List<int> gripIndices = new List<int>();

            foreach (var count in _curveSegmentGripCount)
            {
                gripIndices.Add(cornerIndex);
                cornerIndex += count;
            }

            return gripIndices.ToArray();
        }

        private void UpdateSegments(int gripIndex)
        {
            // trivial case: gripIndex = 0
            if (gripIndex == 0)
            {
                _activePolycurve[gripIndex].SetStartPoint(_editgrips[gripIndex].CurrentLocation);

                if (_polycurveIsClosed)
                {
                    _activePolycurve[_activePolycurve.Length - 1].SetEndPoint(_editgrips[gripIndex].CurrentLocation);
                }

                return;
            }

            // also quite trivial gripIndex = max
            if (gripIndex == _editgrips.Length - 1)
            {
                if(!_polycurveIsClosed)
                _activePolycurve[_activePolycurve.Length - 1].SetEndPoint(_editgrips[gripIndex].CurrentLocation);
                
            }

            // not so trivial

        }

        private void UpdateGrips()
        {
            if (!NewLocation) return;

            int activeGripIndex = -1;

            // find moving grip
            for (int i = 0; i < _editgrips.Length; i++)
            {
                if (_editgrips[i].IsActive && _editgrips[i].Moved) // grip is active and has been moved
                {
                    activeGripIndex = i;
                    for (int j = 0; j < _editgrips.Length; j++)
                    {
                        // disable all other points for the duration of the drag
                        if (!_editgrips[j].Moved) _editgrips[j].IsActive = false;
                    }

                    break;
                }

            }

            if (activeGripIndex == -1) return;

            // change curve segments to new cp location


        }
    }
}
