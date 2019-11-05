using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace PolycurveEditingTools.Core
{
    public class EditGripBase : CustomGripObject
    {
        public EditGripBase()
        {
            IsActive = true;
        }

        public EditGripBase(Point3d originalLocation)
        {
            OriginalLocation = originalLocation;
            IsActive = true;
        }

        public bool IsActive { get; set; }
    }
}