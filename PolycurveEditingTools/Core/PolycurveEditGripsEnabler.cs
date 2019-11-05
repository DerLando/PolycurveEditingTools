using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    internal static class PolycurveEditGripsEnabler
    {
        public static void TurnOnGrips(RhinoObject rhObject)
        {
            // return on null input
            if (rhObject == null) return;

            // try to cast input geometry to polycurve
            var polyCurve = rhObject.Geometry as PolyCurve;
            if (polyCurve == null) return;

            var polycurveEditGrips = new PolycurveEditGrips();
            if(!polycurveEditGrips.CreateGrips(polyCurve)) return;

            rhObject.EnableCustomGrips(polycurveEditGrips);
        }
    }
}
