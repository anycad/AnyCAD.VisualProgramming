using System.IO;
using Dynamo.Interfaces;

namespace AnyCAD.Rapid.Dynamo.Startup
{
    internal class RapidPathResolver : IPathResolver
    {
        private readonly List<string> additionalResolutionPaths;
        private readonly List<string> additionalNodeDirectories;
        private readonly List<string> preloadedLibraryPaths;

        public RapidPathResolver(string preloaderLocation, IEnumerable<string> userNodesDll)
        {
            // If a suitable preloader cannot be found on the system, then do 
            // not add invalid path into additional resolution. The default 
            // implementation of IPathManager in Dynamo insists on having valid 
            // paths specified through "IPathResolver" implementation.
            // 
            additionalResolutionPaths = new List<string>();
            if (Directory.Exists(preloaderLocation))
                additionalResolutionPaths.Add(preloaderLocation);

            additionalNodeDirectories = new List<string>();
            preloadedLibraryPaths =
            [
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DesignScriptBuiltin.dll",
                "DSCoreNodes.dll",
                "DSOffice.dll",
                "DSCPython.dll",
                "FunctionObject.ds",
                "BuiltIn.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                "Analysis.dll",
                "GeometryColor.dll",
                "AnyCAD.CoreNodes.dll",
                .. userNodesDll,
            ];
        }

        public IEnumerable<string> AdditionalResolutionPaths
        {
            get { return additionalResolutionPaths; }
        }

        public IEnumerable<string> AdditionalNodeDirectories
        {
            get { return additionalNodeDirectories; }
        }

        public IEnumerable<string> PreloadedLibraryPaths
        {
            get { return preloadedLibraryPaths; }
        }

        public string UserDataRootFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                "Dynamo", "Dynamo Core").ToString();
            }
        }

        public string CommonDataRootFolder
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Returns the full path of user data location of all version of this
        /// Dynamo product installed on this system. The default implementation
        /// returns list of all subfolders in %appdata%\Dynamo as well as 
        /// %appdata%\Dynamo\Dynamo Core\ folders.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDynamoUserDataLocations()
        {
            var appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dynamoFolder = Path.Combine(appDatafolder, "Dynamo");
            if (!Directory.Exists(dynamoFolder)) return Enumerable.Empty<string>();

            var paths = new List<string>();
            var coreFolder = new FileInfo(UserDataRootFolder).FullName;
            //Dynamo Core folder has to be enumerated first to cater migration from
            //Dynamo 1.0 to Dynamo Core 1.0
            if (Directory.Exists(coreFolder))
            {
                paths.AddRange(Directory.EnumerateDirectories(coreFolder));
            }

            paths.AddRange(Directory.EnumerateDirectories(dynamoFolder));
            return paths;
        }

        /// <summary>
        /// Finds the Dynamo Core path by looking into registery or potentially a config file.
        /// </summary>
        /// <returns>The root folder path of Dynamo Core.</returns>
        public static string GetDynamoCorePath()
        {
            var version = new Version("3.0.3.7597");
            try
            {
                return DynamoInstallDetective.DynamoProducts.GetDynamoPath(version);
            }
            catch
            {
                return null;
            }
        }
    }
}
