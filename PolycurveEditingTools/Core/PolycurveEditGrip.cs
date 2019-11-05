using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class PolycurveEditGrip : EditGripBase
    {
        public PolycurveEditGrip() : base() { }

        public PolycurveEditGrip(Point3d originalLocation) : base(originalLocation) { }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Polycurve edit points" : "Polycurve edit point";
        }
    }
}
