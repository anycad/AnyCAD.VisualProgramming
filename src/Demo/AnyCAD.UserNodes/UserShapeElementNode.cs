using Autodesk.DesignScript.Geometry;
using DynamoServices;
using AnyCAD.Foundation;
using AnyCAD.CoreNodes.Elements;
using AnyCAD.CoreNodes.GeometryInterop;

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

        public static UserShapeElementNode ByExtrudeProfileHeight(Curve profile, double height = 1.0)
        {
            var profileShape = profile.To();
            if (profileShape == null || profileShape.IsNullShape()) 
            {
                throw new ArgumentException("Failed to convert dynamo profile to toposhape");
            }
            var topoShape = FeatureTool.Extrude(profile.To(), height, profile.Normal.To());
            return new UserShapeElementNode(topoShape);
        }
    }
}
