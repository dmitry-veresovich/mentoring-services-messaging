using Core.Messages;

namespace ProcessingServer.Services
{
    internal interface IStatusUpdateService
    {
        void UpdateStatus(StatusType statusType, string additionalInfo = null);
    }
}
