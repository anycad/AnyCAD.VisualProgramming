using Autodesk.DesignScript.Geometry;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.CoreNodes.Extension
{
    [IsVisibleInDynamoLibrary(false)]
    public static class GeometryExtension
    {
        public static GPnt To(this Point pt)
        {
            return new GPnt(pt.X, pt.Y, pt.Z);
        }

        public static TopoShape To(this Geometry geometry)
        {
            // TODO
            return null;
        }
    }
}
