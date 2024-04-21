using System.Windows;

namespace AnyCAD.Rapid.Dynamo.Sandbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AnyCAD.Foundation.Application.Startup();

            // 注册自定义的文档模板
            AnyCAD.Foundation.DocumentManager.Register<RapidDynamoDocTemplate>();
            AnyCAD.Foundation.DocumentManager.Instance().SetDocType(RapidDynamoDocTemplate.DocType);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AnyCAD.Foundation.GlobalInstance.Destroy();
        }
    }

}
