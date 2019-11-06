using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    internal class PolyCurveEditGripsEnabler
    {
        public void TurnOnGrips(RhinoObject rhObject)
        {
            // try to cast input geometry to polycurve
            if (!(rhObject?.Geometry is PolyCurve polyCurve)) return;
            
            var polycurveEditGrips = new PolyCurveEditGrips();
            if (!polycurveEditGrips.CreateGrips(polyCurve)) return;

            rhObject.EnableCustomGrips(polycurveEditGrips);
        }
    }
}
