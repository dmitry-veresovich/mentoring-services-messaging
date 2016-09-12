using System;

namespace Core.Messages
{
    public class SplitMessage<T>
    {
        public T Data { get; set; }
        public Guid Id { get; set; }
        public int Position { get; set; }
        public int Size { get; set; }
    }
}
