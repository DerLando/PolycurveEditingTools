using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class NurbsEditGrip : EditGripBase
    {
        public NurbsEditGrip() : base() { }

        public NurbsEditGrip(Point3d originalLocation) : base(originalLocation) { }

        public override string ShortDescription(bool plural)
        {
            return plural ? "nurbs edit points" : "nurbs edit point";
        }
    }
}
