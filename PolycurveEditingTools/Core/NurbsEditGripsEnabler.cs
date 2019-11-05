using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    internal class NurbsEditGripsEnabler
    {
        public void TurnOnGrips(RhinoObject rhObject)
        {
            // return on null input
            if (rhObject == null) return;

            // try to cast input geometry to nurbs
            var nurbs = rhObject.Geometry as NurbsCurve;
            if (nurbs == null) return;

            var nurbsEditGrips = new NurbsEditGrips();
            if (!nurbsEditGrips.CreateGrips(nurbs)) return;

            rhObject.EnableCustomGrips(nurbsEditGrips);
        }
    }
}
