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
            _editGrips = new ArcEditGrip[5];
            for (int i = 0; i < 5; i++)
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
            var locations = Editor.ArcCurveEditGripLocations(arcCrv);
            for (int i = 0; i < Settings.ArcEditGripCount; i++)
            {
                _editGrips[i] = new ArcEditGrip(locations[i]);
            }

            // Add grips
            for (int i = 0; i < 5; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }

        public int UpdateGrips()
        {
            if (!NewLocation) return -1;

            int activeIndex = -1;

            for (int i = 0; i < 5; i++)
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

            if (activeIndex == -1) return activeIndex;

            // edit arc -> logic in another class
            _activeArc = Editor.EditArc(_activeArc, activeIndex, _editGrips);

            _drawArc = true;
            NewLocation = false;
            return activeIndex;
        }

        protected override void OnReset()
        {
            _drawArc = false;
            _activeArc = _originalArc;
            base.OnReset();
        }

        protected override GeometryBase NewGeometry()
        {
            var activeIndex = UpdateGrips();
            if (GripsMoved && _drawArc)
            {
                // TODO: Set Gumball frame origin to new grip location
                return new ArcCurve(_activeArc);
            }
            return null;
        }

        protected override void OnDraw(GripsDrawEventArgs args)
        {
            UpdateGrips();

            args.DrawControlPolygonLine(_editGrips[0].CurrentLocation, _editGrips[2].CurrentLocation, 0, 2);
            args.DrawControlPolygonLine(_editGrips[1].CurrentLocation, _editGrips[4].CurrentLocation, 1, 4);

            if (_drawArc && args.DrawDynamicStuff)
            {
                args.Display.DrawArc(_activeArc, Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor);
            }
            base.OnDraw(args);
        }

    }
}
