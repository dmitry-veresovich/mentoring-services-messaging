using Core.Data;

namespace ControlServer.Services
{
    interface ISettingsUpdateNotifyService
    {
        void NotifyConfigurationChanged(ProcessingConfiguration configuration);
    }
}
