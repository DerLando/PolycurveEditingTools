using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class NurbsEditGrips : CustomObjectGrips
    {
        private NurbsEditGrip[] _editGrips;
        private Plane _plane;
        private NurbsCurve _activeNurbs;
        private NurbsCurve _originalNurbs;
        private bool _drawNurbs;

        public NurbsEditGrips()
        {
            _drawNurbs = false;
        }

        public bool CreateGrips(NurbsCurve nurbs)
        {
            if (GripCount > 0) return false;

            _originalNurbs = new NurbsCurve(nurbs);
            _activeNurbs = new NurbsCurve(nurbs);

            // set edit grips from arcCrv
            var locations = Editor.NurbsCurveEditGripLocations(nurbs);
            _editGrips = new NurbsEditGrip[locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                _editGrips[i] = new NurbsEditGrip(locations[i]);
            }

            // Add grips
            for (int i = 0; i < _editGrips.Length; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }


    }
}
