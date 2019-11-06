using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public static class Editor
    {
        /// <summary>
        /// Edits a given arc with a given grip
        /// </summary>
        /// <param name="arc">arc to edit</param>
        /// <param name="gripIndex">index of edit grips to use</param>
        /// <param name="grips">all relevant edit grips</param>
        /// <returns></returns>
        public static Arc EditArc(Arc arc, int gripIndex, EditGripBase[] grips)
        {
            if (gripIndex < 0 || gripIndex >= Settings.ArcEditGripCount)
                throw new ArgumentOutOfRangeException($"Editor.EditArc ERROR: Given grip index {gripIndex} was out of range!");

            if (grips.Length != Settings.ArcEditGripCount)
                throw new ArgumentException($"Editor.EditArc ERROR: Grips array was of size {grips.Length} expected array to be of size {Settings.ArcEditGripCount}!");

            Arc result = Arc.Unset;

            // check which edit point is being dragged
            switch (gripIndex)
            {
                case 0:
                    // tangent handle at start
                    result = new Arc(grips[2].CurrentLocation,
                        grips[2].CurrentLocation - grips[0].CurrentLocation, grips[4].CurrentLocation);
                    break;
                case 1:
                    // tangent handle at end
                    result = new Arc(grips[4].CurrentLocation,
                        grips[4].CurrentLocation - grips[1].CurrentLocation, grips[2].CurrentLocation);
                    result = new Arc(result.EndPoint, result.MidPoint, result.StartPoint);
                    break;
                case 2: // point at start of arc
                case 3: // mid-point of arc
                case 4: // point at end of arc
                    result = new Arc(grips[2].CurrentLocation, grips[3].CurrentLocation,
                        grips[4].CurrentLocation);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Calculates the locations of edit points for an arc (in order)
        /// </summary>
        /// <param name="arcCrv">ArcCurve to calculate edit point locations for</param>
        /// <returns>An array of Point3d, one for each edit point</returns>
        public static Point3d[] ArcCurveEditGripLocations(ArcCurve arcCrv)
        {
            var locations = new Point3d[Settings.ArcEditGripCount];
            // set edit grips locations from arcCrv
            locations[0] = new Point3d(arcCrv.PointAtStart - arcCrv.TangentAtStart * Math.Sqrt(arcCrv.GetLength() / 1.0));
            locations[1] = new Point3d(arcCrv.PointAtEnd + arcCrv.TangentAtEnd * Math.Sqrt(arcCrv.GetLength() / 1.0));
            locations[2] = arcCrv.PointAtStart;
            locations[3] = arcCrv.Arc.MidPoint;
            locations[4] = arcCrv.PointAtEnd;

            return locations;
        }

        public static Arc EditArcCurveTooManyGrips(ArcCurve crv, int gripIndex, EditGripBase[] grips)
        {
            // get target location
            var locations = ArcCurveEditGripLocations(crv);

            // empty correct grips array
            var correctGrips = new EditGripBase[locations.Length];
            var tol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var correctGripIndex = -1;

            for (int i = 0; i < correctGrips.Length; i++)
            {
                for (int j = 0; j < grips.Length; j++)
                {
                    if (!locations[i].EpsilonEquals(grips[j].CurrentLocation, tol)) continue;
                    correctGrips[i] = grips[j];
                }
            }

            for (int i = 0; i < correctGrips.Length; i++)
            {
                if (correctGrips[i] != null) continue;

                correctGrips[i] = grips.First(g => g.Moved);
                correctGripIndex = i;
            }

            if(correctGripIndex == -1) throw new ArgumentOutOfRangeException($"Editor.ArcCurveEditTooManyGrips ERROR: Could not determine correct grip index!");

            return EditArc(crv.Arc, correctGripIndex, correctGrips);
        }

        public static Point3d[] LineCurveEditGripLocations(LineCurve lineCrv)
        {
            var locations = new Point3d[Settings.LineEditGripCount];

            locations[0] = new Point3d(lineCrv.PointAtStart - lineCrv.TangentAtStart * Math.Sqrt(lineCrv.GetLength() / 1.0));
            locations[1] = new Point3d(lineCrv.PointAtEnd + lineCrv.TangentAtEnd * Math.Sqrt(lineCrv.GetLength() / 1.0));
            locations[2] = lineCrv.PointAtStart;
            locations[3] = lineCrv.PointAtEnd;

            return locations;
        }

        public static Line EditLine(Line line, int gripIndex, EditGripBase[] grips)
        {
            if (gripIndex < 0 || gripIndex >= Settings.LineEditGripCount)
                throw new ArgumentOutOfRangeException($"Editor.EditLine ERROR: Given grip index {gripIndex} was out of range!");

            if (grips.Length != Settings.LineEditGripCount)
                throw new ArgumentException($"Editor.EditLine ERROR: Grips array was of size {grips.Length} expected array to be of size {Settings.LineEditGripCount}!");

            Line result = Line.Unset;

            // check which edit point is being dragged
            switch (gripIndex)
            {
                case 0:
                    // tangent handle at start
                    result = new Line(grips[2].CurrentLocation, grips[2].CurrentLocation - grips[0].CurrentLocation, line.Length);
                    break;
                case 1:
                    // tangent handle at end
                    result = new Line(grips[3].CurrentLocation, grips[3].CurrentLocation - grips[1].CurrentLocation, line.Length);
                    result = new Line(result.To, result.From);
                    break;
                case 2: // point at start of line
                case 3: // point at end of arc
                    result = new Line(grips[2].CurrentLocation, grips[3].CurrentLocation);
                    break;
            }

            return result;
        }

        public static Line EditLineCurveTooManyGrips(LineCurve crv, int gripIndex, EditGripBase[] grips)
        {
            // get target location
            var locations = LineCurveEditGripLocations(crv);

            // empty correct grips array
            var correctGrips = new EditGripBase[locations.Length];
            var tol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var correctGripIndex = -1;

            for (int i = 0; i < correctGrips.Length; i++)
            {
                for (int j = 0; j < grips.Length; j++)
                {
                    if (!locations[i].EpsilonEquals(grips[j].CurrentLocation, tol)) continue;
                    correctGrips[i] = grips[j];
                }
            }

            for (int i = 0; i < correctGrips.Length; i++)
            {
                if (correctGrips[i] != null) continue;

                correctGrips[i] = grips.First(g => g.Moved);
                correctGripIndex = i;
            }

            if (correctGripIndex == -1) throw new ArgumentOutOfRangeException($"Editor.LineCurveEditTooManyGrips ERROR: Could not determine correct grip index!");

            return EditLine(crv.Line, correctGripIndex, correctGrips);
        }

        public static Point3d[] NurbsCurveEditGripLocations(NurbsCurve nurbs)
        {
            var greville = nurbs.GrevillePoints(false);

            var locations = new Point3d[greville.Count + 2];

            locations[0] = new Point3d(nurbs.PointAtStart - nurbs.TangentAtStart * Math.Sqrt(nurbs.GetLength()));
            locations[1] = new Point3d(nurbs.PointAtEnd + nurbs.TangentAtEnd * Math.Sqrt(nurbs.GetLength()));

            for (int i = 0; i < greville.Count; i++)
            {
                locations[i + 2] = greville[i];
            }

            return locations;
        }

        public static NurbsCurve EditNurbsCurve(NurbsCurve nurbs, int gripIndex, EditGripBase[] grips)
        {
            var targetGripCount = Settings.NurbsEditGripCount(nurbs);

            if (gripIndex < 0 || gripIndex >= targetGripCount)
                throw new ArgumentOutOfRangeException($"Editor.EditNurbsCurve ERROR: Given grip index {gripIndex} was out of range!");

            if (grips.Length != targetGripCount)
                throw new ArgumentException($"Editor.EditNurbsCurve ERROR: Grips array was of size {grips.Length} expected array to be of size {targetGripCount}!");

            var result = new NurbsCurve(nurbs);

            if (gripIndex == 0)
            {
                // tangent handle at start
                var startTangent = grips[2].CurrentLocation - grips[0].CurrentLocation;
                startTangent.Unitize();
                result.SetEndCondition(false, NurbsCurve.NurbsCurveEndConditionType.Tangency, grips[2].CurrentLocation,
                    startTangent);
                return result;
            }

            if (gripIndex == 1)
            {
                // tangent handle at end
                var endTangent = grips[1].CurrentLocation -
                                 grips[targetGripCount - 1].CurrentLocation;
                endTangent.Unitize();
                result.SetEndCondition(true, NurbsCurve.NurbsCurveEndConditionType.Tangency,
                    grips[targetGripCount - 1].CurrentLocation, endTangent);
                return result;
            }

            // greville edit point
            result.SetGrevillePoints(from grip in grips.Skip(2) select grip.CurrentLocation);
            return result;
        }

        public static NurbsCurve EditNurbsCurveTooManyGrips(NurbsCurve crv, int gripIndex, EditGripBase[] grips)
        {
            // get target location
            var locations = NurbsCurveEditGripLocations(crv);

            // empty correct grips array
            var correctGrips = new EditGripBase[locations.Length];
            var tol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var correctGripIndex = -1;

            for (int i = 0; i < correctGrips.Length; i++)
            {
                for (int j = 0; j < grips.Length; j++)
                {
                    if (!locations[i].EpsilonEquals(grips[j].CurrentLocation, tol)) continue;
                    correctGrips[i] = grips[j];
                }
            }

            for (int i = 0; i < correctGrips.Length; i++)
            {
                if (correctGrips[i] != null) continue;

                correctGrips[i] = grips.First(g => g.Moved);
                correctGripIndex = i;
            }

            if (correctGripIndex == -1) throw new ArgumentOutOfRangeException($"Editor.ArcCurveEditTooManyGrips ERROR: Could not determine correct grip index!");

            return EditNurbsCurve(crv, correctGripIndex, correctGrips);
        }

        public static Point3d[] PolyCurveEditGripLocations(PolyCurve curve)
        {
            List<Point3d> locations = new List<Point3d>();

            for (int i = 0; i < curve.SegmentCount; i++)
            {
                var segment = curve.SegmentCurve(i);

                switch (segment.CurveType())
                {
                    case CurveType.LineCurve:
                        var lineCurve = segment as LineCurve;
                        locations.AddRange(LineCurveEditGripLocations(lineCurve).Take(Settings.LineEditGripCount - 1));
                        // omit end point, as start of next curve will be the same location
                        break;
                    case CurveType.ArcCurve:
                        var arcCurve = segment as ArcCurve;
                        locations.AddRange(ArcCurveEditGripLocations(arcCurve).Take(Settings.ArcEditGripCount - 1));
                        break;
                    case CurveType.NurbsCurve:
                        var nurbsCurve = segment as NurbsCurve;
                        var nurbsLocations = NurbsCurveEditGripLocations(nurbsCurve);
                        Array.Resize(ref nurbsLocations, nurbsLocations.Length - 1);
                        locations.AddRange(nurbsLocations);
                        break;
                    case CurveType.Undefined:
                        break;
                }
            }

            // if the input curve is not closed, we need to add the end point of the last segment
            if(!curve.IsClosed) locations.Add(curve.PointAtEnd);

            return locations.ToArray();
        }

        /// <summary>
        /// Gets indices of editgrip array which correspond to `corners`
        /// Corners in this context are the end point of a given segment which is also
        /// the start point of the next segment.
        /// When moving those grips, two curves have to be updated instead of only one.
        /// Start and end grips are omitted, as a corner there can be determined from
        /// the closedness of the polycurve in question.
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private static int[] PolyCurveEditGripsCornerIndices(Curve[] segments)
        {
            int[] cornerIndices = new int[segments.Length - 1];
            int gripCount = 0;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                switch (segments[i].CurveType())
                {
                    case CurveType.LineCurve:
                        gripCount += Settings.LineEditGripCount - 1;
                        break;
                    case CurveType.ArcCurve:
                        gripCount += Settings.ArcEditGripCount - 1;
                        break;
                    case CurveType.NurbsCurve:
                        gripCount += Settings.NurbsEditGripCount(segments[i] as NurbsCurve) - 1;
                        break;
                    case CurveType.Undefined:
                        break;
                }

                cornerIndices[i] = gripCount;
            }

            return cornerIndices;
        }

        private static int[] SegmentIndicesFromEditGripIndex(Curve[] segments, int gripIndex)
        {
            // test if closed
            bool isClosed = segments.Last().PointAtEnd
                .EpsilonEquals(segments[0].PointAtStart, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);

            // trivial cases for closed and open curve
            if (isClosed && gripIndex == 0) return new[] {0, segments.Length - 1};
            if (!isClosed && gripIndex == 0) return new[] {0};

            // write corner index array
            int[] cornerIndices = PolyCurveEditGripsCornerIndices(segments);

            int foundIndex = -1;
            foundIndex = cornerIndices.First(i => i == gripIndex);
            if (foundIndex != -1)
            {
                // gripIndex matches a corner, return segment indices for corner
                return new[] {foundIndex, foundIndex + 1};
            }
            else
            {
                for (int i = 0; i < cornerIndices.Length; i++)
                {
                    if (gripIndex > cornerIndices[i]) continue;
                    return new[] {i};
                }
            }

            throw new ArgumentException("Could not determine segment indices :(");

        }

        public static PolyCurve EditPolyCurve(PolyCurve curve, int gripIndex, PolyCurveEditGrip[] grips)
        {
            var targetGripCount = grips.Length;

            if (gripIndex < 0 || gripIndex >= targetGripCount)
                throw new ArgumentOutOfRangeException($"Editor.EditPolyCurve ERROR: Given grip index {gripIndex} was out of range!");

            if (grips.Length != targetGripCount)
                throw new ArgumentException($"Editor.EditPolyCurve ERROR: Grips array was of size {grips.Length} expected array to be of size {targetGripCount}!");

            var segments = curve.DuplicateSegments();

            // check trivial gripcount
            if (gripIndex == grips.Length - 1 && !curve.IsClosed)
            {
                curve.SetEndPoint(grips[gripIndex].CurrentLocation);
                return curve;
            }

            if (gripIndex == 2)
            {
                curve.SetStartPoint(grips[gripIndex].CurrentLocation);
                return curve;
            }

            bool lastSegment = true;
            int gripCount = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                int curGripCount = 0;

                switch (segments[i].CurveType())
                {
                    case CurveType.LineCurve:
                        curGripCount = Settings.LineEditGripCount - 1;
                        if (gripIndex >= gripCount && gripIndex <= gripCount + curGripCount)
                        {
                            segments[i] = new LineCurve(EditLineCurveTooManyGrips(segments[i] as LineCurve, gripIndex, grips));
                            //var editGrips = grips.Skip(gripCount).Take(curGripCount).ToList();
                            //editGrips.Add(grips[gripCount + curGripCount + 2]);
                            
                            //segments[i] = new LineCurve(EditLine((segments[i] as LineCurve).Line, gripIndex - gripCount,
                            //    editGrips.ToArray()));

                            lastSegment = false;
                        }
                        break;
                    case CurveType.ArcCurve:
                        curGripCount = Settings.ArcEditGripCount - 1;
                        if (gripIndex >= gripCount && gripIndex <= gripCount + curGripCount)
                        {
                            segments[i] = new ArcCurve(EditArcCurveTooManyGrips(segments[i] as ArcCurve, gripIndex, grips));
                            //var editGrips = grips.Skip(gripCount).Take(curGripCount).ToList();
                            //editGrips.Add(grips[gripCount + curGripCount + 2]);

                            //segments[i] = new ArcCurve(EditArc((segments[i] as ArcCurve).Arc, gripIndex - gripCount,
                            //    editGrips.ToArray()));

                            lastSegment = false;
                        }
                        break;
                    case CurveType.NurbsCurve:
                        curGripCount = Settings.NurbsEditGripCount(segments[i] as NurbsCurve) - 1;
                        if (gripIndex >= gripCount && gripIndex <= gripCount + curGripCount)
                        {
                            segments[i] = EditNurbsCurveTooManyGrips(segments[i] as NurbsCurve, gripIndex, grips);
                            //var editGrips = grips.Skip(gripCount).Take(curGripCount).ToList();
                            //editGrips.Add(grips[gripCount + curGripCount + 2]);

                            //segments[i] = EditNurbsCurve(segments[i] as NurbsCurve, gripIndex - gripCount,
                            //    editGrips.ToArray());

                            lastSegment = false;
                        }
                        break;
                    case CurveType.Undefined:
                        break;
                }

                gripCount += curGripCount;
            }

            if (lastSegment)
            {
                int lastIndex = segments.Length - 1;
                switch (segments[lastIndex].CurveType())
                {
                    case CurveType.LineCurve:
                        break;
                    case CurveType.ArcCurve:
                        break;
                    case CurveType.NurbsCurve:
                        break;
                    case CurveType.PolylineCurve:
                        break;
                    case CurveType.Undefined:
                        break;
                }
            }

            var joined = Curve.JoinCurves(segments, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            if(joined.Length != 1) throw new ArgumentOutOfRangeException($"Editor.EditPolyCurveERROR: Segments could not be joined after editing!");
            return joined[0] as PolyCurve;
        }
    }
}
