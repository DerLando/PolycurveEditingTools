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
        public static Arc EditArc(Arc arc, int gripIndex, ArcEditGrip[] grips)
        {
            if (gripIndex < 0 || gripIndex >= Settings.ArcEditGripCount)
                throw new ArgumentOutOfRangeException($"Editor.EditArc ERROR: Given grip index {gripIndex} was out of range!");

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
    }
}
