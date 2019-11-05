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
            _editGrips[0] = new ArcEditGrip(new Point3d(arcCrv.PointAtStart - arcCrv.TangentAtStart * Math.Sqrt(arcCrv.GetLength() / 1.0)));
            _editGrips[1] = new ArcEditGrip(arcCrv.PointAtStart);
            _editGrips[2] = new ArcEditGrip(arcCrv.Arc.MidPoint);
            _editGrips[3] = new ArcEditGrip(arcCrv.PointAtEnd);
            _editGrips[4] = new ArcEditGrip(new Point3d(arcCrv.PointAtEnd + arcCrv.TangentAtEnd * Math.Sqrt(arcCrv.GetLength() / 1.0)));

            // Add grips
            for (int i = 0; i < 5; i++)
            {
                AddGrip(_editGrips[i]);
            }

            return true;
        }

        public void UpdateGrips()
        {
            if (!NewLocation) return;

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

            // check which edit point is being dragged
            switch (activeIndex)
            {
                case -1:
                    return;
                case 0:
                    // tangent handle at start
                    _activeArc = new Arc(_editGrips[1].CurrentLocation,
                        _editGrips[1].CurrentLocation - _editGrips[0].CurrentLocation, _editGrips[3].CurrentLocation);
                    break;
                case 1:
                    // point at start of arc
                    _activeArc = new Arc(_editGrips[1].CurrentLocation, _editGrips[2].CurrentLocation,
                        _editGrips[3].CurrentLocation);
                    break;
                case 2:
                    // mid-point of arc
                    _activeArc = new Arc(_editGrips[1].CurrentLocation, _editGrips[2].CurrentLocation,
                        _editGrips[3].CurrentLocation);
                    break;
                case 3:
                    // point at end of arc
                    _activeArc = new Arc(_editGrips[1].CurrentLocation, _editGrips[2].CurrentLocation,
                        _editGrips[3].CurrentLocation);
                    break;
                case 4:
                    // tangent handle at end
                    _activeArc = new Arc(_editGrips[3].CurrentLocation,
                        _editGrips[3].CurrentLocation - _editGrips[4].CurrentLocation, _editGrips[1].CurrentLocation);
                    _activeArc = new Arc(_activeArc.EndPoint, _activeArc.MidPoint, _activeArc.StartPoint);
                    break;
                default:
                    throw new ArgumentException($"{activeIndex} was not between -1 and 3 :(");
            }

            _drawArc = true;
            NewLocation = false;
        }

        protected override void OnReset()
        {
            _drawArc = false;
            _activeArc = _originalArc;
            base.OnReset();
        }

        protected override GeometryBase NewGeometry()
        {
            UpdateGrips();
            if (GripsMoved && _drawArc) return new ArcCurve(_activeArc);
            return null;
        }

        protected override void OnDraw(GripsDrawEventArgs args)
        {
            UpdateGrips();
            if (_drawArc && args.DrawDynamicStuff)
            {
                args.Display.DrawArc(_activeArc, Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor);
                args.DrawControlPolygonLine(_editGrips[0].CurrentLocation, _editGrips[1].CurrentLocation, 0, 1);
                args.DrawControlPolygonLine(_editGrips[4].CurrentLocation, _editGrips[3].CurrentLocation, 4, 3);
            }
            base.OnDraw(args);
        }
    }
}
