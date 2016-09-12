using System.Collections.Generic;

namespace Core.Messages
{
    public interface IMessagesService<T>
    {
        IEnumerable<SplitMessage<T>> Split(T data);
        T Collect(IEnumerable<SplitMessage<T>> messages);
    }

    public interface IBytesMessagesService : IMessagesService<byte[]>
    {
    }
}
