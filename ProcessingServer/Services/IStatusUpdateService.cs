using Core.Messages;

namespace ProcessingServer.Services
{
    public interface IStatusUpdateService
    {
        void UpdateStatus(StatusType statusType, string additionalInfo = null);
    }
}
