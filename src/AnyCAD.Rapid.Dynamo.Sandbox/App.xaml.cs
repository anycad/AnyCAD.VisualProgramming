using System.Configuration;
using System.Data;
using System.Windows;

namespace AnyCAD.Rapid.Dynamo.SandBox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AnyCAD.Foundation.Application.Startup();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AnyCAD.Foundation.GlobalInstance.Destroy();
        }
    }

}
