using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.Geometry
{
    public class TopoShapeNode
    {
        private TopoShape mTopoShape;
        private TopoShapeNode(TopoShape shape)
        {
            mTopoShape = shape;
        }

        [IsVisibleInDynamoLibrary(false)]
        public TopoShape Value => mTopoShape;

        public static TopoShapeNode ByGCicle(GCircleNode circle)
        {
            if (circle == null)
            {
                throw new ArgumentNullException(nameof(circle));
            }
            return new TopoShapeNode(circle.TopoShape);
        }
    }
}
