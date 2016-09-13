using Core.Data;

namespace Core.Messages
{
    public class UpdateStatusMessage
    {
        public StatusType StatusType { get; set; }
        public string AdditionalInfo { get; set; }
        public ProcessingConfiguration ProcessingConfiguration { get; set; }
    }
}
