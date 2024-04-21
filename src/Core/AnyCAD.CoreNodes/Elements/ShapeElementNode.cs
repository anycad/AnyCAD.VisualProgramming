using Autodesk.DesignScript.Geometry;
using DynamoServices;

using AnyCAD.Foundation;

using AnyCAD.Rapid.Dynamo.Services.Persistence;
using AnyCAD.CoreNodes.GeometryInterop;
using AnyCAD.CoreNodes.Geometry;

using ADGeometry = Autodesk.DesignScript.Geometry.Geometry;

namespace AnyCAD.CoreNodes.Elements
{
    /// <summary>
    /// ShapeElementNode，AnyCAD.Foundation.ShapeElement在Dynamo环境下的适配器
    /// 扩展用于创建ShapeElement的节点时从该类继承并增加自定义的By...静态方法
    /// </summary>
    [RegisterForTrace]
    public class ShapeElementNode : ElementNode
    {
        internal ShapeElement InternalShape { get; private set; }
        public override Element InternalElement => InternalShape;

        #region Constructors
        protected ShapeElementNode(TopoShape topoShape)
        {
            SafeInit(() => Init(topoShape));
        }

        public static ShapeElementNode ByGeometry(ADGeometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            var topoShape = geometry.To();
            if (topoShape == null)
            {
                throw new ArgumentNullException(nameof(topoShape));
            }

            return new ShapeElementNode(topoShape);
        }

        public static ShapeElementNode ByTopoShape(TopoShapeNode topo)
        {
            if (topo == null)
            {
                throw new ArgumentNullException(nameof(topo));
            }

            return new ShapeElementNode(topo.Value);
        }
        #endregion

        #region Setters
        /// <summary>
        /// 设置物体颜色
        /// </summary>
        /// <param name="color"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetColor(DSCore.Color color)
        {
            var shapeElement = InternalShape;
            if (shapeElement == null)
            {
                throw new ArgumentNullException(nameof(InternalShape));
            }

            UndoTransaction undo = new(Document);
            undo.Start("update");
            var tuple = color.To();
            shapeElement.SetColor(tuple.Item1);
            undo.Commit();
        }

        /// <summary>
        /// 设置物体材质
        /// </summary>
        /// <param name="material"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetMaterial(MaterialNode material)
        {
            var shapeElement = InternalShape;
            if (shapeElement == null)
            {
                throw new ArgumentNullException(nameof(InternalShape));
            }

            var materialElement = material.InternalMaterial;
            if (materialElement == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            UndoTransaction undo = new(Document);
            undo.Start("update");
            shapeElement.SetMaterialId(materialElement.GetId());
            undo.Commit();
        }
        #endregion

        #region PrivateUtils
        private void Init(TopoShape topoShape)
        {
            // try to reuse from trace
            var shapeElement = ShapeElement.Cast(ElementBinder.GetElementFromTrace(Document));

            UndoTransaction undo = new(Document);
            undo.Start("shape");
            if (shapeElement == null)
            {
                shapeElement = ShapeElement.Create(Document);
            }
            shapeElement.SetShape(topoShape);
            undo.Commit();

            InternalInit(shapeElement);

            // TODO: clear and set?
            ElementBinder.SetElementForTrace(shapeElement.GetId());
        }

        private void InternalInit(ShapeElement shapeElement)
        {
            InternalShape = shapeElement;
            InternalId = shapeElement.GetId();
        }
        #endregion
    }
}
