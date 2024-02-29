using Autodesk.DesignScript.Runtime;

using AnyCAD.Foundation;

using AnyCAD.Rapid.Dynamo.Services.Elements;
using AnyCAD.Rapid.Dynamo.Services.Persistence;

namespace AnyCAD.CoreNodes.Elements
{
    /// <summary>
    /// ElementNode，AnyCAD.Foundation.Element在Dynamo环境下的适配器
    /// 实现了与AnyCAD文档对象之间的绑定和生命周期管理
    /// </summary>
    public abstract class ElementNode : IDisposable
    {
        protected void SafeInit(Action init)
        {
            try
            {
                init();
            }
            catch (Exception e)
            {
                // TODO
                throw e;
            }
        }

        internal static Document Document => Application.Instance().GetActiveDocument();

        /// <summary>
        /// 若当前对象已经被anycad所拥有，那么在Dispose中不应被删除
        /// </summary>
        internal bool _isOwned = false;

        [SupressImportIntoVM]
        public abstract Element InternalElement { get; }

        private ObjectId _internalId;

        protected ObjectId InternalId
        {
            get
            {
                if (_internalId == null || _internalId.IsInvalid())
                    return InternalElement != null ? InternalElement.GetId() : ObjectId.InvalidId;
                return _internalId;
            }
            set
            {
                _internalId = value;

                var manager = ElementIdLifecycleManager.Instance;
                manager.RegisterAsssociation(Id, this);
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public virtual void Dispose()
        {
            if (DisposeLogic.IsShuttingDown || DisposeLogic.IsClosingHomeworkspace)
                return;

            var manager = ElementIdLifecycleManager.Instance;
            // bool didOwnerDelete = manager.IsOwnerDeleted(Id);

            int remainingBindings = manager.UnRegisterAssociation(Id, this);

            if (!_isOwned && remainingBindings == 0 && InternalId.IsValid())
            {
                UndoTransaction undo = new(Document);
                undo.Start("erase");
                Document.RemoveElement(InternalId);
                undo.Commit();
            }
            else
            {
                //This element has gone
                //but there was something else holding onto the AnyCAD object so don't purge it

                _internalId = ObjectId.InvalidId;
            }
        }

        public ulong Id
        {
            get
            {
                var id = InternalId;
                if (id.IsInvalid())
                    return 0;
                return id.GetInteger();
            }
        }
    }
}
