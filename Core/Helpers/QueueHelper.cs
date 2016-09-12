using System.Messaging;

namespace Core.Helpers
{
    public static class QueueHelper
    {
        public static void EnsureQueueExists(string path)
        {
            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path);
        }
    }
}
