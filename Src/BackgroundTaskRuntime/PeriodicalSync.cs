using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundTaskRuntime
{
    public class PeriodicalSync : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private CustomMiBand _customMiBand;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            _customMiBand = null;

            try
            {
                Debug.WriteLine($"MESSAGE: BackgroundTask: {typeof(NotificationChanged).FullName} running!");

                _customMiBand = new CustomMiBand();
                await _customMiBand.UpdateOperations();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                _customMiBand?.Dispose();
                _customMiBand = null;

                _deferral.Complete();
            }
        }
    }
}
