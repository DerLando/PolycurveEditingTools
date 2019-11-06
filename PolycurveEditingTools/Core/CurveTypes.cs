using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public enum CurveType
    {
        LineCurve,
        ArcCurve,
        NurbsCurve,
        PolylineCurve,
        Undefined,
    }

    public static class CurveTypes
    {
        public static CurveType CurveType(this Curve crv)
        {
            var type = crv.GetType();

            if (type == typeof(LineCurve)) return Core.CurveType.LineCurve;
            if (type == typeof(ArcCurve)) return Core.CurveType.ArcCurve;
            if (type == typeof(PolylineCurve)) return Core.CurveType.PolylineCurve;
            if (type == typeof(NurbsCurve)) return Core.CurveType.NurbsCurve;
            return Core.CurveType.Undefined;
        }
    }
}
