using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.ViewModels.Watch3D;

namespace AnyCAD.Rapid.Dynamo.Startup
{
    internal class RapidDynamoViewModel : DynamoViewModel
    {
        #region Constructors
        protected RapidDynamoViewModel(StartConfiguration startConfiguration) : base(startConfiguration)
        {

        }

        public new static RapidDynamoViewModel Start(StartConfiguration startConfiguration)
        {
            if (startConfiguration.DynamoModel == null)
                startConfiguration.DynamoModel = DynamoModel.Start();

            if (startConfiguration.WatchHandler == null)
                startConfiguration.WatchHandler = new DefaultWatchHandler(startConfiguration.DynamoModel.PreferenceSettings);

            if (startConfiguration.Watch3DViewModel == null)
            {
                startConfiguration.Watch3DViewModel =
                    HelixWatch3DViewModel.TryCreateHelixWatch3DViewModel(
                        null,
                        new Watch3DViewModelStartupParams(startConfiguration.DynamoModel),
                        startConfiguration.DynamoModel.Logger);
            }

            return new RapidDynamoViewModel(startConfiguration);
        }
        #endregion
    }
}
