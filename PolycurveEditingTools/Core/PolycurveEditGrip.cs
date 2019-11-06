using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class PolyCurveEditGrip : EditGripBase
    {
        public PolyCurveEditGrip() : base() { }

        public PolyCurveEditGrip(Point3d originalLocation) : base(originalLocation) { }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Polycurve edit points" : "Polycurve edit point";
        }
    }
}
