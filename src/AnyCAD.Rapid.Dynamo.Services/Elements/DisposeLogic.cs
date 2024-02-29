using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.Rapid.Dynamo.Services.Elements
{
    /// <summary>
    /// DisposeLogic
    /// </summary>
    internal class DisposeLogic
    {
        /// <summary>
        /// This flag is set to true in the event that you want to
        /// notify IDisposable objects whether Dynamo is shutting down.
        /// Ex. When shutting down AnyCAD, we set IsShuttingDown to true,
        /// and we read this flag in the Dispose method. For AbstractElement
        /// elements this allows us to bypass cleanup of AnyCAD elements.
        /// </summary>
        public static bool IsShuttingDown { get; set; }

        /// <summary>
        /// This flag is set to true when Dynamo is going to close
        /// current home workspace and set to false when home workspace is
        /// removed.
        /// </summary>
        public static bool IsClosingHomeworkspace { get; set; }
    }
}
