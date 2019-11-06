using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class PolyCurveEditGrips : CustomObjectGrips
    {
        private PolyCurveEditGrip[] _editGrips;
        private Plane _plane;
        private PolyCurve _activePolycurve;
        private PolyCurve _originalPolycurve;
        private int[] _curveSegmentGripCount;
        private bool _drawPolycurve;
        private bool _polycurveIsClosed;

        public PolyCurveEditGrips()
        {
            _drawPolycurve = false;
        }

        public bool CreateGrips(PolyCurve polyCurve)
        {
            // if grips are already created, return
            if (GripCount > 0) return false;

            // generate active and original curve arrays from input
            _originalPolycurve = polyCurve.DuplicatePolyCurve();
            _activePolycurve = polyCurve.DuplicatePolyCurve();
            _polycurveIsClosed = polyCurve.IsClosed;

            // List to hold all editgrips
            var locations = Editor.PolyCurveEditGripLocations(_originalPolycurve);
            _editGrips = new PolyCurveEditGrip[locations.Length];
            for (int i = 0; i < _editGrips.Length; i++)
            {
                _editGrips[i] = new PolyCurveEditGrip(locations[i]);
            }

            // Add grips
            for (int i = 0; i < _editGrips.Length; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }


        private int[] GetCornerGripIndices()
        {
            int cornerIndex = 0;
            List<int> gripIndices = new List<int>();

            foreach (var count in _curveSegmentGripCount)
            {
                gripIndices.Add(cornerIndex);
                cornerIndex += count;
            }

            return gripIndices.ToArray();
        }

        private void UpdateGrips()
        {
            if (!NewLocation) return;

            int activeGripIndex = -1;

            // find moving grip
            for (int i = 0; i < _editGrips.Length; i++)
            {
                if (_editGrips[i].IsActive && _editGrips[i].Moved) // grip is active and has been moved
                {
                    activeGripIndex = i;
                    for (int j = 0; j < _editGrips.Length; j++)
                    {
                        // disable all other points for the duration of the drag
                        if (!_editGrips[j].Moved) _editGrips[j].IsActive = false;
                    }

                    break;
                }

            }

            if (activeGripIndex == -1) return;

            // change curve segments to new cp location
            _activePolycurve = Editor.EditPolyCurve(_activePolycurve, activeGripIndex, _editGrips);

        }
    }
}
