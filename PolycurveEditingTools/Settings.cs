using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace PolycurveEditingTools
{
    public static class Settings
    {
        public static int ArcEditGripCount = 5;
        public static int LineEditGripCount = 4;
        public static int NurbsEditGripCount(NurbsCurve nurbs) => nurbs.GrevillePoints(false).Count + 2;
    }
}
