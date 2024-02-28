using Autodesk.DesignScript.Geometry;
using DynamoServices;

using AnyCAD.Foundation;

using AnyCAD.Rapid.Dynamo.Services.Persistence;
using AnyCAD.CoreNodes.Extension;

namespace AnyCAD.CoreNodes.Elements
{
    [RegisterForTrace]
    public class ShapeElementNode : ElementNode
    {
        internal ShapeElement InternalShape { get; private set; }
        public override Element InternalElement => InternalShape;

        protected ShapeElementNode(TopoShape topoShape)
        {
            SafeInit(() => Init(topoShape));
        }

        public static ShapeElementNode ByGeometry(Geometry geometry)
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

        protected void Init(TopoShape topoShape)
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
    }
}
