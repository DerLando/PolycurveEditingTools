﻿using System;
using System.Runtime.InteropServices;
using PolycurveEditingTools.Core;
using PolycurveEditingTools.Getters;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Commands
{
    [Guid("BC64AD46-A6B8-403B-AC83-D6EA92E4BAA8")]
    public class pePointsOn : Command
    {
        private PolyCurveEditGripsEnabler _polyGripsEnabler;
        private ArcEditGripsEnabler _arcGripsEnabler;
        private NurbsEditGripsEnabler _nurbsGripsEnabler;
        private LineEditGripsEnabler _lineGripsEnabler;

        static pePointsOn _instance;
        public pePointsOn()
        {
            _instance = this;
        }

        ///<summary>The only instance of the pePointsOn command.</summary>
        public static pePointsOn Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "pePointsOn"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (_polyGripsEnabler is null)
            {
                _polyGripsEnabler = new PolyCurveEditGripsEnabler();
                CustomObjectGrips.RegisterGripsEnabler(_polyGripsEnabler.TurnOnGrips, typeof(PolyCurveEditGrips));
            }

            if (_arcGripsEnabler is null)
            {
                _arcGripsEnabler = new ArcEditGripsEnabler();
                CustomObjectGrips.RegisterGripsEnabler(_arcGripsEnabler.TurnOnGrips, typeof(ArcEditGrips));
            }

            if (_nurbsGripsEnabler is null)
            {
                _nurbsGripsEnabler = new NurbsEditGripsEnabler();
                CustomObjectGrips.RegisterGripsEnabler(_nurbsGripsEnabler.TurnOnGrips, typeof(NurbsEditGrips));
            }

            if (_lineGripsEnabler is null)
            {
                _lineGripsEnabler = new LineEditGripsEnabler();
                CustomObjectGrips.RegisterGripsEnabler(_lineGripsEnabler.TurnOnGrips, typeof(LineEditGrips));
            }

            var go = new GetPolycurve();
            go.SetCommandPrompt("Select curves for point display");
            go.GetMultiple(1, 0);
            if (go.CommandResult() != Result.Success) return go.CommandResult();

            for (int i = 0; i < go.ObjectCount; i++)
            {
                var rhObject = go.Object(i).Object();
                if (rhObject != null)
                {
                    var crv = rhObject.Geometry as Curve;
                    var type = crv.GetType();
                    if (type == typeof(ArcCurve))
                    {
                        if (rhObject.GripsOn) rhObject.GripsOn = false;
                        _arcGripsEnabler.TurnOnGrips(rhObject);
                        continue;
                    }

                    if (type == typeof(PolyCurve))
                    {
                        if (rhObject.GripsOn) rhObject.GripsOn = false;
                        _polyGripsEnabler.TurnOnGrips(rhObject);
                        continue;
                    }

                    if (type == typeof(NurbsCurve))
                    {
                        if (rhObject.GripsOn) rhObject.GripsOn = false;
                        _nurbsGripsEnabler.TurnOnGrips(rhObject);
                        continue;
                    }

                    if (type == typeof(LineCurve))
                    {
                        if (rhObject.GripsOn) rhObject.GripsOn = false;
                        _lineGripsEnabler.TurnOnGrips(rhObject);
                        continue;
                    }
                }
            }

            doc.Views.Redraw();

            return Result.Success;
        }
    }
}