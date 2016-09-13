using Core.Messages;

namespace ControlServer.Services
{
    internal interface IUpdateStatusLogService
    {
        void LogStatus(UpdateStatusMessage updateStatusMessage);
    }
}
