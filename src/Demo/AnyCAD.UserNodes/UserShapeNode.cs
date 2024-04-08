using Autodesk.DesignScript.Geometry;
using DynamoServices;
using AnyCAD.Foundation;
using AnyCAD.CoreNodes.Elements;
using AnyCAD.CoreNodes.GeometryInterop;
using AnyCAD.CoreNodes.Geometry;
using Autodesk.DesignScript.Runtime;

namespace AnyCAD.UserNodes.Elements
{
    /// <summary>
    /// 用户自定义的ShapeElement节点创建方式
    /// </summary>
    [RegisterForTrace]
    public class UserShapeNode : ShapeElementNode
    {
        private UserShapeNode(TopoShape topoShape)
            : base(topoShape)
        {

        }

        /// <summary>
        /// 通过输入球心和半径创建形状为球体的ShapeElement
        /// </summary>
        /// <param name="center">GPntNode节点类型定义的球心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static UserShapeNode BySphereCenterRadius([DefaultArgument("GPntNode.ByCoordinates(0, 0, 0)")] GPntNode center, 
                                                                    double radius = 1.0)
        {
            var topoShape = ShapeBuilder.MakeSphere(center.Value, radius);
            return new UserShapeNode(topoShape);
        }

        /// <summary>
        /// 通过输入球心和半径创建形状为球体的ShapeElement
        /// </summary>
        /// <param name="center">Point节点类型定义的球心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static UserShapeNode BySphereCenterRadius([DefaultArgument("Point.ByCoordinates(0, 0, 0)")] Point center, 
                                                                    double radius = 1.0)
        {
            return BySphereCenterRadius(GPntNode.ByNativePoint(center), radius);
        }

        /// <summary>
        /// 通过输入长方体的轴和边长创建形状为长方体的ShapeElement
        /// </summary>
        /// <param name="ax">GAx2Node节点类型定义的长方体轴向</param>
        /// <param name="dx">长方体在X轴方向的长度</param>
        /// <param name="dy">长方体在Y轴方向的长度</param>
        /// <param name="dz">长方体在Z轴方向的长度</param>
        /// <returns>返回生成的长方体ShapeElement</returns>
        public static UserShapeNode ByBoxAxisLengths([DefaultArgument("CoordinateSystem.ByOrigin(0, 0)")] CoordinateSystem cs, 
                                                            double dx = 1.0, 
                                                            double dy = 1.0, 
                                                            double dz = 1.0)
        {
            // 从输入节点获取轴向和边长
            var ax2 = cs.To();
            var lengthX = dx;
            var lengthY = dy;
            var lengthZ = dz;

            // 使用ShapeBuilder API创建长方体形状
            var topoShape = ShapeBuilder.MakeBox(ax2, lengthX, lengthY, lengthZ);

            // 将创建的长方体形状封装为Dynamo的UserShapeNode
            return new UserShapeNode(topoShape);
        }

        /// <summary>
        /// 通过输入圆柱的轴向、半径、高度和可选的开角创建形状为圆柱的ShapeElement
        /// </summary>
        /// <param name="ax">CoordinateSystem节点类型定义的圆柱轴向</param>
        /// <param name="radius">圆柱的半径</param>
        /// <param name="height">圆柱的高度</param>
        /// <param name="angle">圆柱的开角，0表示闭合圆柱</param>
        /// <returns>返回生成的圆柱ShapeElement</returns>
        public static UserShapeNode ByCylinderAxisRadiusHeight([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 1)")] CoordinateSystem ax,
                                                                        double radius = 1.0,
                                                                        double height = 1.0,
                                                                        double angle = 0.0)
        {
            // 从输入节点获取轴向，以及其他参数
            var ax2 = ax.To();
            var r = radius;
            var h = height;
            var a = angle;

            // 使用ShapeBuilder API创建圆柱形状
            var topoShape = ShapeBuilder.MakeCylinder(ax2, r, h, a);

            // 将创建的圆柱形状封装为Dynamo的UserShapeNode
            return new UserShapeNode(topoShape);
        }

        /// <summary>
        /// 通过输入圆锥的轴向、底部半径、顶部半径、高度和开角创建形状为圆锥的ShapeElement
        /// </summary>
        /// <param name="ax">CoordinateSystem节点类型定义的圆锥轴向</param>
        /// <param name="radius">圆锥的底部半径</param>
        /// <param name="radiusTop">圆锥的顶部半径，如果为0则表示圆锥尖端闭合</param>
        /// <param name="height">圆锥的高度</param>
        /// <param name="angle">圆锥的开角</param>
        /// <returns>返回生成的圆锥ShapeElement</returns>
        public static UserShapeNode ByConeAxisRadiusHeight([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 1)")] CoordinateSystem ax,
                                                                    double radius = 1.0,
                                                                    double radiusTop = 0.0,
                                                                    double height = 1.0,
                                                                    double angle = 0.0)
        {
            // 从输入节点获取轴向，以及其他参数
            var ax2 = ax.To();
            var r = radius;
            var rTop = radiusTop;
            var h = height;
            var a = angle;

            // 使用ShapeBuilder API创建圆锥形状
            var topoShape = ShapeBuilder.MakeCone(ax2, r, rTop, h, a);

            // 将创建的圆锥形状封装为Dynamo的UserShapeNode
            return new UserShapeNode(topoShape);
        }

        /// <summary>
        /// 通过输入圆环的轴向、主半径、次半径和开角创建形状为圆环的ShapeElement
        /// </summary>
        /// <param name="majorR">圆环的主半径，即圆环的中心到圆环上任意点的距离</param>
        /// <param name="minorR">圆环的次半径，即圆环上任意点到圆环中心的最小距离</param>
        /// <param name="ax">CoordinateSystem节点类型定义的圆环轴向</param>
        /// <param name="angle">圆环的开角，使用弧度制</param>
        /// <returns>返回生成的圆环ShapeElement</returns>
        public static UserShapeNode ByTorusMajorMinorAngle([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 1)")] CoordinateSystem ax,
                                                                    double majorR = 2.0,
                                                                    double minorR = 1.0,
                                                                    double angle = Math.PI * 2)
        {
            // 从输入节点获取轴向，以及其他参数
            var ax2 = ax.To();
            var mr = majorR;
            var mrTop = minorR;
            var a = angle;

            // 使用ShapeBuilder API创建圆环形状
            var topoShape = ShapeBuilder.MakeTorus(mr, mrTop, ax2, a);

            // 将创建的圆环形状封装为Dynamo的UserShapeNode
            return new UserShapeNode(topoShape);
        }

        /// <summary>
        /// 通过输入截面和高度创建拉伸体形状的ShapeElement，拉伸方向为截面轮廓法向
        /// </summary>
        /// <param name="profile">Curve类型定义的截面轮廓</param>
        /// <param name="height">拉伸高度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static UserShapeNode ByExtrudeProfileHeight(Curve profile, double height = 1.0)
        {
            var profileShape = profile.To();
            if (profileShape == null || profileShape.IsNullShape()) 
            {
                throw new ArgumentException("Failed to convert dynamo profile to toposhape");
            }
            var topoShape = FeatureTool.Extrude(profile.To(), height, profile.Normal.To());
            return new UserShapeNode(topoShape);
        }
    }
}
