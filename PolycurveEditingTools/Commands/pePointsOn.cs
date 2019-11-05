using System;
using System.Runtime.InteropServices;
using PolycurveEditingTools.Core;
using PolycurveEditingTools.Getters;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects.Custom;

namespace PolycurveEditingTools.Commands
{
    [Guid("BC64AD46-A6B8-403B-AC83-D6EA92E4BAA8")]
    public class pePointsOn : Command
    {
        private PolycurveEditGripsEnabler _gripsEnabler;

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
            if (_gripsEnabler is null)
            {
                _gripsEnabler = new PolycurveEditGripsEnabler();
                CustomObjectGrips.RegisterGripsEnabler(_gripsEnabler.TurnOnGrips, typeof(PolycurveEditGrips));
            }

            var go = new GetPolycurve();
            go.SetCommandPrompt("Select Polycurves for point display");
            go.GetMultiple(1, 0);
            if (go.CommandResult() != Result.Success) return go.CommandResult();

            for (int i = 0; i < go.ObjectCount; i++)
            {
                var rhObject = go.Object(i).Object();
                if (rhObject != null)
                {
                    if (rhObject.GripsOn) rhObject.GripsOn = false;
                    _gripsEnabler.TurnOnGrips(rhObject);
                }
            }

            doc.Views.Redraw();

            return Result.Success;
        }
    }
}