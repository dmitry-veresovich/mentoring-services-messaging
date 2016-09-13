using System;
using System.IO;
using System.Xml.Serialization;
using Core.Data;
using Core.Messages;

namespace ControlServer.Services
{
    class UpdateStatusLogService : IUpdateStatusLogService
    {
        private const string Log = "statusLog.xml";

        public void LogStatus(UpdateStatusMessage updateStatusMessage)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Log);
            var serializer = new XmlSerializer(typeof(LogEntry[]));
            LogEntry[] allMessages;

            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                LogEntry[] oldMessages = null;
                try
                {
                    oldMessages = serializer.Deserialize(fileStream) as LogEntry[];
                }
                catch (InvalidOperationException)
                {
                    // File is invalid or empty.
                }

                var oldLength = oldMessages?.Length ?? 0;
                var newLength = oldLength + 1;
                allMessages = new LogEntry[newLength];
                if (oldMessages != null && oldMessages.Length > 0)
                {
                    Array.Copy(oldMessages, 0, allMessages, 1, oldLength);
                }

                allMessages[0] = LogEntry.Create(updateStatusMessage);
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(fileStream, allMessages);
            }
        }
    }

    public class LogEntry
    {
        public static LogEntry Create(UpdateStatusMessage message)
        {
            return new LogEntry
            {
                StatusType = message.StatusType,
                AdditionalInfo = message.AdditionalInfo,
                ProcessingConfiguration = message.ProcessingConfiguration,
                Time = DateTime.Now.ToLongTimeString(),
            };
        }

        public StatusType StatusType { get; set; }
        public string AdditionalInfo { get; set; }
        public ProcessingConfiguration ProcessingConfiguration { get; set; }
        public string Time { get; set; }
    }
}
