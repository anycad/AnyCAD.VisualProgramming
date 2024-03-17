using AnyCAD.CoreNodes.GeometryInterop;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.CoreNodes.Geometry
{
    /// <summary>
    /// AnyCAD提供的几何点对象
    /// </summary>
    public class GPntNode : IGraphicItem
    {
        private GPnt mPnt;
        private GPntNode(double x, double y, double z)
        {
            mPnt = new GPnt(x, y, z);
        }
        private GPntNode(GPnt pnt)
        {
            mPnt = pnt;
        }

        [IsVisibleInDynamoLibrary(false)]
        public GPnt Value => mPnt;

        public double X => mPnt.x;
        public double Y => mPnt.y;
        public double Z => mPnt.z;

        public static GPntNode ByCoordinates(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            return new GPntNode(x, y, z);
        }

        public static GPntNode ByNativePoint(Autodesk.DesignScript.Geometry.Point pt)
        {
            return new GPntNode(pt.To());
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            if (mPnt == null)
                return;
            package.AddPointVertex(mPnt.x, mPnt.y, mPnt.z);
        }
    }
}
