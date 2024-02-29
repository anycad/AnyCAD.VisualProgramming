using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using AnyCAD.WPF;
using AnyCAD.Foundation;
using AnyCAD.Rapid.Dynamo.Startup;

namespace AnyCAD.Rapid.Dynamo.Sandbox
{
    public partial class MainViewModel
    {
        private RenderControl mRenderControl;
        private MainDocumentListener mDocumentListener;

        public MainViewModel(RenderControl renderControl)
        {
            mRenderControl = renderControl;
        }

        public Document Document => Application.Instance().GetActiveDocument();

        public bool Initialize()
        {
            if (mRenderControl == null)
                return false;

            var viewer = mRenderControl.Viewer;
            var viewContext = mRenderControl.ViewContext;
            Application.Instance().SetActiveViewer(viewer);

            var smgr = viewContext.GetSelectionManager();
            smgr.SetDepthTest(true);
            smgr.AddListener(new SelectionListener(viewer));

            mDocumentListener = new MainDocumentListener(this);
            DocumentEvent.Instance().AddListener(mDocumentListener);
            Document document = Application.Instance().CreateDocument(".acad");
            document.SetModified(false);
            Application.Instance().SetActiveDocument(document);
            return true;
        }

        public void OnDocumentChanged(DocumentEventArgs args)
        {
            ViewContext viewContext = mRenderControl.ViewContext;
            if (viewContext == null || args.GetPreviewing())
            {
                return;
            }

            Scene scene = viewContext.GetScene();
            foreach (ObjectId addedId in args.GetAddedIds())
            {
                ulong integer = addedId.GetInteger();
                if (scene.FindNodeByUserId(integer) == null)
                {
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            foreach (ObjectId removedId in args.GetRemovedIds())
            {
                ulong integer2 = removedId.GetInteger();
                SceneNode sceneNode = scene.FindNodeByUserId(integer2);
                if (sceneNode != null)
                {
                    scene.RemoveNode(sceneNode.GetUuid());
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            Document document = Document;
            foreach (ObjectId changedId in args.GetChangedIds())
            {
                ObjectId objectId = changedId;
                ulong value = changedId.Value;
                Component component = Component.Cast(document.FindElement(changedId));
                if (component != null)
                {
                    objectId = component.GetEntityId();
                    value = objectId.Value;
                }

                if (scene.FindNodeByUserId(value) != null)
                {
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }
        }

        [RelayCommand]
        void OnOpenDynamo()
        {
            var userNodesDll = new List<string>()
            {
                "AnyCAD.UserNodes.dll"
            };
            var layoutSpecs = GetLayoutSpecsPath();
            RapidDynamoManager.Instance.ConfigureUserNodes(userNodesDll, layoutSpecs);
            RapidDynamoManager.Instance.StartDynamo();
        }

        private string? GetLayoutSpecsPath()
        {
            var asmLocation = Assembly.GetExecutingAssembly().Location;
            var asmDirectory = System.IO.Path.GetDirectoryName(asmLocation);
            return System.IO.Path.Combine(asmDirectory, "Resources", "LayoutSpecs.json");
        }
    }
}
