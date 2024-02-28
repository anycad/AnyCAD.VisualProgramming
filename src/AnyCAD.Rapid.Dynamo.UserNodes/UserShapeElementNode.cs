using Autodesk.DesignScript.Geometry;

using AnyCAD.Foundation;
using AnyCAD.Rapid.Dynamo.CoreNodes.Elements;
using AnyCAD.Rapid.Dynamo.CoreNodes.Extension;

namespace UserNodes
{
    public class UserShapeElementNode : ShapeElementNode
    {
        private UserShapeElementNode(TopoShape topoShape)
            : base(topoShape)
        {

        }

        public static UserShapeElementNode BySphereCenterRadius(Point center, double radius = 1.0)
        {
            var topoShape = ShapeBuilder.MakeSphere(center.To(), radius);
            return new UserShapeElementNode(topoShape);
        }
    }
}
