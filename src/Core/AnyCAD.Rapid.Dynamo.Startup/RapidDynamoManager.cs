using Dynamo.Controls;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using DyViews = Dynamo.UI.Views;

using System.IO;
using System.Reflection;
using System.Windows;

namespace AnyCAD.Rapid.Dynamo.Startup
{
    /// <summary>
    /// RapidDynamoManager，用于管理AnyCAD/Dynamo调用的单例
    /// </summary>
    public class RapidDynamoManager
    {
        private static RapidDynamoManager mInstance;

        /// <summary>
        /// 获取单例的Instance
        /// </summary>
        public static RapidDynamoManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new();
                return mInstance;
            }
        }

        private RapidDynamoManager()
        {
            
        }

        private DyViews.SplashScreen mSplashScreen;
        private IEnumerable<string> mUserNodesDll = [];
        private string mUserLayoutSpecs;
        private Window mParentWindow;
        /// <summary>
        /// 启动Dynamo窗口
        /// </summary>
        /// <returns></returns>
        public bool StartDynamo(Window parentWindow)
        {
            mParentWindow = parentWindow;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly; // TODO: unregister when closing?

            mSplashScreen = new();
            mSplashScreen.DynamicSplashScreenReady += LoadDynamoView;
            mSplashScreen.Show();

            return true;
        }

        /// <summary>
        /// 配置AnyCAD扩展节点，仅当自定义了扩展节点库时进行设置
        /// </summary>
        /// <param name="userNodesDll">包含扩展节点定义的类库dll名称列表</param>
        /// <param name="userLayoutSpecs">扩展节点目录树结构的配置文件路径</param>
        public void ConfigureUserNodes(IEnumerable<string> userNodesDll, string userLayoutSpecs)
        {
            mUserNodesDll = userNodesDll;
            mUserLayoutSpecs = userLayoutSpecs;
        }

        private DynamoViewModel? mDynamoViewModel;
        private DynamoView? mDynamoView;
        private DynamoModel? mDynamoModel;
        private void LoadDynamoView()
        {
            mDynamoModel = RapidDynamoModel.Start(mUserNodesDll);
            mDynamoViewModel = RapidDynamoViewModel.Start(
                   new DynamoViewModel.StartConfiguration()
                   {
                       CommandFilePath = string.Empty,
                       DynamoModel = mDynamoModel,
                       ShowLogin = false,
                   });

            mDynamoView = new DynamoView(mDynamoViewModel);
            mDynamoView.Loaded += (o, e) => UpdateLibraryLayoutSpec(mUserLayoutSpecs);
            mDynamoView.Show();
            mDynamoView.Owner = mParentWindow;
            mDynamoView.Activate();

            mSplashScreen.DynamicSplashScreenReady -= LoadDynamoView;
            mSplashScreen.Close();
        }

        /// <summary>
        /// Updates the Libarary Layout spec to include layout for AnyCAD nodes. 
        /// The AnyCAD layout spec is embeded as resource "LayoutSpecs.json".
        /// </summary>
        private void UpdateLibraryLayoutSpec(string userLayoutSpecs)
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

            //Extend it with the layoutSpecs user provided
            if (File.Exists(userLayoutSpecs))
            {
                try
                {
                    var userspec = LayoutSpecification.FromJSONString(File.ReadAllText(userLayoutSpecs));
                    var userSection = userspec.sections.First();
                    var userCategroy = userSection.childElements.First();

                    var anycadCategoryToExtend = elements.First(elem => elem.text == "AnyCAD");
                    anycadCategoryToExtend?.childElements.AddRange(userCategroy.childElements);
                }
                catch (Exception)
                {
                    Console.WriteLine(string.Format("Exception while trying to load {0}", userLayoutSpecs));
                }
            }


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
        private Assembly? ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {  
                var assemblyDirectory = GetExecutingAssemblyDirectory();

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

        private string? GetExecutingAssemblyDirectory()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
