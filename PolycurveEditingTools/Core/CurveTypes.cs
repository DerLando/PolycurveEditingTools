using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public enum CurveType
    {
        LineCurve,
        ArcCurve,
        NurbsCurve,
        Undefined,
    }

    public static class CurveTypes
    {
        public static CurveType CurveType(this Curve crv)
        {
            var type = crv.GetType();

            if (type == typeof(LineCurve)) return Core.CurveType.LineCurve;
            if (type == typeof(ArcCurve)) return Core.CurveType.ArcCurve;
            return Core.CurveType.NurbsCurve;
        }
    }
}
