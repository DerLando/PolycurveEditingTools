using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    internal class ArcEditGripsEnabler
    {
        public void TurnOnGrips(RhinoObject rhObject)
        {
            // return on null input
            if (rhObject == null) return;

            // try to cast input geometry to arccurve
            var arcCurve = rhObject.Geometry as ArcCurve;
            if (arcCurve == null) return;
            
            var arcEditGrips = new ArcEditGrips();
            if (!arcEditGrips.CreateGrips(arcCurve)) return;

            rhObject.EnableCustomGrips(arcEditGrips);
        }
    }
}
