using System.Diagnostics;
using System.IO;
using System.Messaging;
using Core.Messages;
using MigraDoc.Rendering;
using ProcessingServer.Configuration;

namespace ProcessingServer.Services
{
    class MessagingImagePersistenceService : IImagePersistenceService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IBytesMessagesService _bytesMessagesService;

        public MessagingImagePersistenceService(IConfigurationProvider configurationProvider,
            IBytesMessagesService bytesMessagesService)
        {
            _configurationProvider = configurationProvider;
            _bytesMessagesService = bytesMessagesService;
        }

        public void Persist(PdfDocumentRenderer renderer)
        {
            using (var queue = new MessageQueue(_configurationProvider.CentralServerQueue))
            {
                using (var stream = new MemoryStream())
                {
                    renderer.Save(stream, true);
                    var bytes = stream.ToArray();
                    var messages = _bytesMessagesService.Split(bytes);
                    foreach (var message in messages)
                    {
                        TrySend(queue, message);
                    }
                }
            }
        }

        private static void TrySend(MessageQueue queue, SplitMessage<byte[]> message)
        {
            try
            {
                queue.Send(message);
            }
            catch (MessageQueueException e)
            {
                Debug.Write(e);
                throw;
            }
        }
    }
}
