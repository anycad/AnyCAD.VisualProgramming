using Autodesk.DesignScript.Geometry;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.CoreNodes.GeometryInterop
{
    [IsVisibleInDynamoLibrary(false)]
    public static class BasicGeometryConvert
    {
        public static GPnt To(this Point pt)
        {
            return new GPnt(pt.X, pt.Y, pt.Z);
        }

        public static GDir To(this Vector vec)
        {
            return new GDir(vec.X, vec.Y, vec.Z);
        }

        public static TopoShape To(this Geometry geometry)
        {
            // TODO
            if (geometry is Curve crv)
            {
                return crv.To();
            }    
            throw new NotImplementedException();
        }
    }
}
