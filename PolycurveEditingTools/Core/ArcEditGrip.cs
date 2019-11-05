using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    class ArcEditGrip : CustomGripObject
    {
        public bool IsActive { get; set; }

        public ArcEditGrip()
        {
            IsActive = true;
        }

        public ArcEditGrip(Point3d originalLocation)
        {
            OriginalLocation = originalLocation;
            IsActive = true;
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Arc edit points" : "Arc edit point";
        }
    }
}
