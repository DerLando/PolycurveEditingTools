using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class ArcEditGrips : CustomObjectGrips
    {
        private readonly ArcEditGrip[] _editGrips;
        private Plane _plane;
        private Arc _activeArc;
        private Arc _originalArc;
        private bool _drawArc;

        public ArcEditGrips()
        {
            _drawArc = false;
            _editGrips = new ArcEditGrip[3];
            for (int i = 0; i < 3; i++)
            {
                _editGrips[i] = new ArcEditGrip();
            }
        }

        public bool CreateGrips(ArcCurve arcCrv)
        {
            // return if already grips present
            if (GripCount > 0) return false;

            // copy arc into private fields
            _originalArc = arcCrv.Arc;
            _activeArc = arcCrv.Arc;

            // set edit grips from arcCrv
            _editGrips[0] = new ArcEditGrip(arcCrv.PointAtStart);
            _editGrips[1] = new ArcEditGrip(arcCrv.Arc.MidPoint);
            _editGrips[2] = new ArcEditGrip(arcCrv.PointAtEnd);

            // Add grips
            for (int i = 0; i < 3; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }

    }
}
