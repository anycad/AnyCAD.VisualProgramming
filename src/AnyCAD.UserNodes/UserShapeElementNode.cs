using Autodesk.DesignScript.Geometry;
using DynamoServices;
using AnyCAD.Foundation;
using AnyCAD.CoreNodes.Elements;
using AnyCAD.CoreNodes.Extension;

namespace AnyCAD.UserNodes.Elements
{
    [RegisterForTrace]
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
