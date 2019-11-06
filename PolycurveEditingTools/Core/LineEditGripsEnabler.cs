using Rhino.DocObjects;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    internal class LineEditGripsEnabler
    {
        public void TurnOnGrips(RhinoObject rhObject)
        {
            // return on null input
            if (rhObject == null) return;

            // try to cast input geometry to Linecurve
            var LineCurve = rhObject.Geometry as LineCurve;
            if (LineCurve == null) return;
            
            var LineEditGrips = new LineEditGrips();
            if (!LineEditGrips.CreateGrips(LineCurve)) return;

            rhObject.EnableCustomGrips(LineEditGrips);
        }
    }
}
