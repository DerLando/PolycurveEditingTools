using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class LineEditGrips : CustomObjectGrips
    {
        private readonly LineEditGrip[] _editGrips;
        private Plane _plane;
        private Line _activeLine;
        private Line _originalLine;
        private bool _drawLine;

        public LineEditGrips()
        {
            _drawLine = false;
            _editGrips = new LineEditGrip[Settings.LineEditGripCount];
            for (int i = 0; i < Settings.LineEditGripCount; i++)
            {
                _editGrips[i] = new LineEditGrip();
            }
        }

        public bool CreateGrips(LineCurve LineCrv)
        {
            // return if already grips present
            if (GripCount > 0) return false;

            // copy Line into private fields
            _originalLine = LineCrv.Line;
            _activeLine = LineCrv.Line;

            // set edit grips from LineCrv
            var locations = Editor.LineCurveEditGripLocations(LineCrv);
            for (int i = 0; i < Settings.LineEditGripCount; i++)
            {
                _editGrips[i] = new LineEditGrip(locations[i]);
            }

            // Add grips
            for (int i = 0; i < Settings.LineEditGripCount; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }

        public int UpdateGrips()
        {
            if (!NewLocation) return -1;

            int activeIndex = -1;

            for (int i = 0; i < Settings.LineEditGripCount; i++)
            {
                if (_editGrips[i].IsActive && _editGrips[i].Moved)
                {
                    for (int j = 0; j < Settings.LineEditGripCount; j++)
                    {
                        if (!_editGrips[j].Moved) _editGrips[j].IsActive = false;
                    }

                    activeIndex = i;
                    break;
                }
            }

            if (activeIndex == -1)
                return activeIndex;

            // edit Line -> logic in another class
            _activeLine = Editor.EditLine(_activeLine, activeIndex, _editGrips);

            _drawLine = true;
            NewLocation = false;
            return activeIndex;
        }

        protected override void OnReset()
        {
            _drawLine = false;
            _activeLine = _originalLine;
            base.OnReset();
        }

        protected override GeometryBase NewGeometry()
        {
            var activeIndex = UpdateGrips();
            if (GripsMoved && _drawLine)
            {
                // TODO: Set Gumball frame origin to new grip location
                return new LineCurve(_activeLine);
            }
            return null;
        }

        protected override void OnDraw(GripsDrawEventArgs args)
        {
            UpdateGrips();

            args.DrawControlPolygonLine(_editGrips[0].CurrentLocation, _editGrips[2].CurrentLocation, 0, 2);
            args.DrawControlPolygonLine(_editGrips[1].CurrentLocation, _editGrips[3].CurrentLocation, 1, 3);

            if (_drawLine && args.DrawDynamicStuff)
            {
                args.Display.DrawLine(_activeLine, Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor);
            }
            base.OnDraw(args);
        }

    }
}
