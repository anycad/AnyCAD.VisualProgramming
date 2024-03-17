using AnyCAD.Foundation;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using AnyCAD.CoreNodes.GeometryInterop;

namespace AnyCAD.CoreNodes.Geometry
{
    /// <summary>
    /// AnyCAD提供的几何圆曲线对象
    /// </summary>
    public class GCircleNode : IGraphicItem
    {
        private GCirc mCirc;
        private GCircleNode(GAx2 position, double radius)
        {
            mCirc = new(position, radius);
        }
        private GCircleNode(GCirc circ)
        {
            mCirc = circ;
        }

        public double Radius => mCirc.Radius();
        public GPntNode Center => GPntNode.ByCoordinates(mCirc.Location().x, mCirc.Location().y, mCirc.Location().z);

        [IsVisibleInDynamoLibrary(false)]
        public TopoShape TopoShape => CurveBuilder.MakeCircle(mCirc);

        public static GCircleNode ByCenterPointRadius([DefaultArgument("GPntNode.ByCoordinates(0, 0, 0)")] GPntNode center, double radius = 1.0)
        {
            GAx2 position = new(center.Value, new GDir(0, 0, 1), new GDir(1, 0, 0));
            return new GCircleNode(position, radius);
        }

        public static GCircleNode ByNativeCircle(Circle circle)
        {
            return new GCircleNode(circle.To());
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            if (mCirc == null)
                return;

            ParametricCurve pc = new(TopoShape);
            if (!pc.IsValidGeometry())
                return;

            double radius = mCirc.Radius();
            int segNum = Math.Max(20, (int)(radius / 0.025));
            double fp = pc.FirstParameter();
            double lp = pc.LastParameter();
            double deltaP = (lp - fp) / segNum;
            for (int i = 0; i <= segNum; i++)
            {
                var p = pc.Value(fp + i * deltaP);
                if (p == null)
                    return;
                package.AddLineStripVertex(p.x, p.y, p.z);
            }
            package.AddLineStripVertexCount(segNum + 1);

            // TODO: 复用AnyCAD的离散化
            //var bsNode = BrepSceneNode.Create(circShape);
            //if (bsNode == null)
            //    return;
            //// var sc = circShape.Write();
            //var bufferShape = BufferShape.Cast(bsNode.GetShape());
            //if (bufferShape == null)
            //    return;

            //var edges = bufferShape.GetEdges();
            //foreach (var edge in edges)
            //{
            //    var pos = edge.GetPositions();
            //    var data = new Float32Array(pos.GetData());
            //    var vertexCount = data.GetItemCount() / 3;
            //    for (uint i = 0; i < vertexCount; i++)
            //    {
            //        var v = data.GetVec3(i);
            //        package.AddLineStripVertex(v.X, v.Y, v.Z);
            //    }
            //    package.AddLineStripVertexCount((int)vertexCount);
            //}
        }
    }
}
