using Autodesk.DesignScript.Geometry;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.CoreNodes.GeometryInterop
{
    [IsVisibleInDynamoLibrary(false)]
    public static class BasicGeometryConvert
    {
        /// <summary>
        /// Dynamo点对象转换
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static GPnt To(this Point pt)
        {
            return new GPnt(pt.X, pt.Y, pt.Z);
        }

        /// <summary>
        /// Dynamo向量对象转换
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static GDir To(this Vector vec)
        {
            return new GDir(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Dynamo几何对象转换
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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
