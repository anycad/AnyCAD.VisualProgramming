using Dynamo.Controls;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AnyCAD.Rapid.Dynamo.Startup
{
    public class RapidDynamoManager
    {
        private static RapidDynamoManager mInstance;
        public static RapidDynamoManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new();
                return mInstance;
            }
        }

        private string mDynamoCorePath;
        private RapidDynamoManager()
        {
            
        }

        private DynamoViewModel? mDynamoViewModel;
        private DynamoView? mDynamoView;
        private DynamoModel? mDynamoModel;

        public bool StartDynamo(IEnumerable<string> userNodesDll)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly; // TODO: unregister when closing?

            mDynamoModel = RapidDynamoModel.Start(userNodesDll);

            mDynamoViewModel = RapidDynamoViewModel.Start(
                   new DynamoViewModel.StartConfiguration()
                   {
                       CommandFilePath = string.Empty,
                       DynamoModel = mDynamoModel,
                       ShowLogin = false,
                   });

            mDynamoView = new DynamoView(mDynamoViewModel);
            mDynamoView.Loaded += (o, e) => UpdateLibraryLayoutSpec();
            mDynamoView.Show();
            mDynamoView.Activate();
            return true;
        }

        /// <summary>
        /// Updates the Libarary Layout spec to include layout for AnyCAD nodes. 
        /// The AnyCAD layout spec is embeded as resource "LayoutSpecs.json".
        /// </summary>
        private void UpdateLibraryLayoutSpec()
        {
            //Get the library view customization service to update spec
            var customization = mDynamoModel.ExtensionManager.Service<ILibraryViewCustomization>();
            if (customization == null) return;

            //Register the icon resource
            customization.RegisterResourceStream("/icons/Category.AnyCAD.svg",
                GetResourceStream("AnyCAD.Rapid.Dynamo.Startup.Resources.Category.AnyCAD.svg"));

            //Read the anycadspec from the resource stream
            LayoutSpecification anycadspec;
            using (Stream stream = GetResourceStream("AnyCAD.Rapid.Dynamo.Startup.Resources.LayoutSpecs.json"))
            {
                anycadspec = LayoutSpecification.FromJSONStream(stream);
            }

            //The anycadspec should have only one section, add all its child elements to the customization
            var elements = anycadspec.sections.First().childElements;
            customization.AddElements(elements); //add all the elements to default section
        }

        /// <summary>
        /// Reads the embeded resource stream by given name
        /// </summary>
        /// <param name="resource">Fully qualified name of the embeded resource.</param>
        /// <returns>The resource Stream if successful else null</returns>
        private Stream GetResourceStream(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }

        /// <summary>
        /// Handler to the ApplicationDomain's AssemblyResolve event.
        /// If an assembly's location cannot be resolved, an exception is
        /// thrown. Failure to resolve an assembly will leave Dynamo in 
        /// a bad state, so we should throw an exception here which gets caught 
        /// by our unhandled exception handler and presents the crash dialogue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Assembly? ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {  
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                var cultureName = Thread.CurrentThread.CurrentCulture.Name;
                var candidates = Directory.GetFiles(assemblyDirectory, assemblyName, SearchOption.AllDirectories);
                string? assemblyPath = candidates.FirstOrDefault((string candidate) =>
                    {
                        return candidate.Contains(cultureName);
                    }, candidates.FirstOrDefault());

                return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyName), ex);
            }
        }
    }
}
