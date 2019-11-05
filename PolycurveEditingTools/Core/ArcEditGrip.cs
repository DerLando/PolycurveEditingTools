using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class ArcEditGrip : EditGripBase
    {
        public ArcEditGrip() : base()
        {
        }

        public ArcEditGrip(Point3d originalLocation) : base(originalLocation)
        {
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Arc edit points" : "Arc edit point";
        }
    }
}
