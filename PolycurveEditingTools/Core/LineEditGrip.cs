using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class LineEditGrip : EditGripBase
    {
        public LineEditGrip() : base()
        {
        }

        public LineEditGrip(Point3d originalLocation) : base(originalLocation)
        {
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Line edit points" : "Line edit point";
        }
    }
}
