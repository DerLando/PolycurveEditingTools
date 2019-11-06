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

        public int UpdateGrips()
        {
            if (!NewLocation) return -1;

            int activeIndex = -1;

            for (int i = 0; i < _editGrips.Length; i++)
            {
                if (_editGrips[i].IsActive && _editGrips[i].Moved)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (!_editGrips[j].Moved) _editGrips[j].IsActive = false;
                    }

                    activeIndex = i;
                    break;
                }
            }

            // edit nurbs -> logic in another class
            _activeNurbs = Editor.EditNurbsCurve(_activeNurbs, activeIndex, _editGrips);

            _drawNurbs = true;
            NewLocation = false;
            return activeIndex;
        }

        protected override void OnReset()
        {
            _drawNurbs = false;
            _activeNurbs = new NurbsCurve(_originalNurbs);
            base.OnReset();
        }

        protected override GeometryBase NewGeometry()
        {
            var activeIndex = UpdateGrips();
            if (GripsMoved && _drawNurbs)
            {
                // TODO: Set Gumball frame origin to new grip location
                return _activeNurbs;
            }
            return null;
        }

        protected override void OnDraw(GripsDrawEventArgs args)
        {
            UpdateGrips();

            args.DrawControlPolygonLine(_editGrips[0].CurrentLocation, _editGrips[2].CurrentLocation, 0, 2);
            var end = 1;
            var start = _editGrips.Length - 1;
            args.DrawControlPolygonLine(_editGrips[end].CurrentLocation, _editGrips[start].CurrentLocation, end, start);

            if (_drawNurbs && args.DrawDynamicStuff)
            {
                args.Display.DrawCurve(_activeNurbs, Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor);
            }
            base.OnDraw(args);
        }


    }
}
