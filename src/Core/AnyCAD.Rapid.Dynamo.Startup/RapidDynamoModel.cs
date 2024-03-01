using System.Reflection;
using System.IO;

using DyConfiguration = Dynamo.Configuration;
using Dynamo.Models;
using Dynamo.Applications;

using AnyCAD.Rapid.Dynamo.Services.Elements;
using Dynamo.Graph.Workspaces;

namespace AnyCAD.Rapid.Dynamo.Startup
{
    internal class RapidDynamoModel : DynamoModel
    {
        #region Constructors
        private RapidDynamoModel(IStartConfiguration config) : base(config)
        {
            DisposeLogic.IsShuttingDown = false;
        }

        public static RapidDynamoModel Start(IEnumerable<string> userNodesDll)
        {
            var isASMloaded = PreloadASM(string.Empty, out string geometryFactoryPath, out string preloaderLocation);
            var config = new DefaultStartConfiguration
            {
                GeometryFactoryPath = geometryFactoryPath,
                Context = DyConfiguration.Context.NONE,
                PathResolver = new RapidPathResolver(preloaderLocation, userNodesDll),
                // TODO: others
            };
            return new RapidDynamoModel(config);
        }
        #endregion

        #region Events
        protected override void OnWorkspaceRemoveStarted(WorkspaceModel workspace)
        {
            base.OnWorkspaceRemoveStarted(workspace);

            if (workspace is HomeWorkspaceModel)
                DisposeLogic.IsClosingHomeworkspace = true;
        }

        protected override void OnWorkspaceRemoved(WorkspaceModel workspace)
        {
            base.OnWorkspaceRemoved(workspace);

            if (workspace is HomeWorkspaceModel)
                DisposeLogic.IsClosingHomeworkspace = false;

            //Unsubscribe the event
            //foreach (var node in workspace.Nodes.ToList())
            //{
            //    node.PropertyChanged -= node_PropertyChanged;
            //}
        }
        #endregion

        #region Utilities
        protected override void ShutDownCore(bool shutDownHost)
        {
            DisposeLogic.IsShuttingDown = true;

            base.ShutDownCore(shutDownHost);
        }

        private static bool PreloadASM(string asmPath, out string geometryFactoryPath, out string preloaderLocation)
        {
            if (string.IsNullOrEmpty(asmPath)/* && OSHelper.IsWindows()*/)
            {
                geometryFactoryPath = string.Empty;
                preloaderLocation = string.Empty;
                try
                {
                    StartupUtils.PreloadShapeManager(ref geometryFactoryPath, ref preloaderLocation);
                }
                catch (Exception e)
                {
                    // ASMPreloadFailure?.Invoke(e.Message);
                    return false;
                }
                // If the output locations are not valid, return false
                if (!Directory.Exists(preloaderLocation) && !File.Exists(geometryFactoryPath))
                {
                    return false;
                }
                return true;
            }

            // get sandbox executing location - this is where libG will be located.
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // defaults - preload these will fail.
            preloaderLocation = "libg_0_0_0";
            geometryFactoryPath = Path.Combine(preloaderLocation, DynamoShapeManager.Utilities.GeometryFactoryAssembly);

            try
            {
                if (!Directory.Exists(asmPath))
                {
                    throw new FileNotFoundException($"{nameof(asmPath)}:{asmPath}");
                }
                Version asmBinariesVersion = DynamoShapeManager.Utilities.GetVersionFromPath(asmPath, /*OSHelper.IsWindows() ? */"*ASMAHL*.dll"/* : "*ASMahl*.so"*/);

                //get version of libG that matches the asm version that was supplied from geometryLibraryPath.
                preloaderLocation = DynamoShapeManager.Utilities.GetLibGPreloaderLocation(asmBinariesVersion, rootFolder);
                geometryFactoryPath = Path.Combine(preloaderLocation, DynamoShapeManager.Utilities.GeometryFactoryAssembly);

                //load asm and libG.
                DynamoShapeManager.Utilities.PreloadAsmFromPath(preloaderLocation, asmPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("A problem occurred while trying to load ASM or LibG");
                Console.WriteLine($"{e?.Message} : {e?.StackTrace}");
                return false;
            }
        }
        #endregion
    }
}
