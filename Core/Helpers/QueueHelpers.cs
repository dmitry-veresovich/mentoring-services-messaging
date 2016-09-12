using System.Messaging;

namespace Core.Helpers
{
    public static class QueueHelpers
    {
        public static void EnsureQueueExists(string path)
        {
            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path);
        }
    }
}
