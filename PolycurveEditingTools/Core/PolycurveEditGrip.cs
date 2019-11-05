using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;

namespace PolycurveEditingTools.Core
{
    public class PolycurveEditGrip : CustomGripObject
    {
        public bool IsActive { get; set; }

        public PolycurveEditGrip()
        {
            IsActive = true;
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Polycurve edit points" : "Polycurve edit point";
        }
    }
}
