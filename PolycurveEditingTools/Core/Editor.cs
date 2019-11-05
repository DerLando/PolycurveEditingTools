using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static Arc EditArc(Arc arc, int gripIndex, ArcEditGrip[] grips)
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
                    result = new Arc(grips[1].CurrentLocation,
                        grips[1].CurrentLocation - grips[0].CurrentLocation, grips[3].CurrentLocation);
                    break;
                case 1:
                    // point at start of arc
                    result = new Arc(grips[1].CurrentLocation, grips[2].CurrentLocation,
                        grips[3].CurrentLocation);
                    break;
                case 2:
                    // mid-point of arc
                    result = new Arc(grips[1].CurrentLocation, grips[2].CurrentLocation,
                        grips[3].CurrentLocation);
                    break;
                case 3:
                    // point at end of arc
                    result = new Arc(grips[1].CurrentLocation, grips[2].CurrentLocation,
                        grips[3].CurrentLocation);
                    break;
                case 4:
                    // tangent handle at end
                    result = new Arc(grips[3].CurrentLocation,
                        grips[3].CurrentLocation - grips[4].CurrentLocation, grips[1].CurrentLocation);
                    result = new Arc(result.EndPoint, result.MidPoint, result.StartPoint);
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
            locations[1] = arcCrv.PointAtStart;
            locations[2] = arcCrv.Arc.MidPoint;
            locations[3] = arcCrv.PointAtEnd;
            locations[4] = new Point3d(arcCrv.PointAtEnd + arcCrv.TangentAtEnd * Math.Sqrt(arcCrv.GetLength() / 1.0));

            return locations;
        }

        public static Point3d[] NurbsCurveEditGripLocations(NurbsCurve nurbs)
        {
            var greville = nurbs.GrevillePoints(false);

            var locations = new Point3d[greville.Count + 2];

            locations[0] = new Point3d(nurbs.PointAtStart - nurbs.TangentAtStart * Math.Sqrt(nurbs.GetLength()));
            locations[locations.Length - 1] = new Point3d(nurbs.PointAtEnd + nurbs.TangentAtEnd * Math.Sqrt(nurbs.GetLength()));

            for (int i = 0; i < greville.Count; i++)
            {
                locations[i + 1] = greville[i];
            }

            return locations;
        }
    }
}
