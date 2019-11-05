using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;

namespace PolycurveEditingTools.Getters
{
    public class GetPolycurve : GetObject
    {
        public override bool CustomGeometryFilter(RhinoObject rhObject, GeometryBase geometry, ComponentIndex componentIndex)
        {
            var type = geometry.GetType();
            return type == typeof(PolyCurve) | type == typeof(ArcCurve) | type == typeof(NurbsCurve);
        }
    }
}
