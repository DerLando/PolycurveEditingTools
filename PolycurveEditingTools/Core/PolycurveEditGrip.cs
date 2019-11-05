using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class PolycurveEditGrip : CustomGripObject
    {
        public bool IsActive { get; set; }

        public PolycurveEditGrip()
        {
            IsActive = true;
        }

        public PolycurveEditGrip(Point3d originalLocation)
        {
            OriginalLocation = originalLocation;
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Polycurve edit points" : "Polycurve edit point";
        }
    }
}
