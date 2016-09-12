using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Messages
{
    public class BytesMessagesService : IBytesMessagesService
    {
        private const int MessageLimit = 100000; // MSMQ limit is 4MB, but for testing purposes it is less.

        public IEnumerable<SplitMessage<byte[]>> Split(byte[] data)
        {
            var guid = Guid.NewGuid();
            var size = (int)Math.Ceiling(data.Length / (double) MessageLimit);

            for (int position = 0; position < size; position++)
            {
                byte[] chunk = new byte[MessageLimit];

                var startIndex = position*MessageLimit;
                var copySize = GetCopySize(data, startIndex);
                Array.Copy(data, startIndex, chunk, 0, copySize);

                yield return new SplitMessage<byte[]>
                {
                    Id = guid,
                    Data = chunk,
                    Position = position,
                    Size = size,
                };
            }
        }

        private static int GetCopySize(byte[] data, int startIndex)
        {
            var leftToCopy = data.Length - startIndex;
            return leftToCopy > MessageLimit ? MessageLimit : leftToCopy;
        }

        public byte[] Collect(IEnumerable<SplitMessage<byte[]>> messages)
        {
            var bytes = new List<byte>();

            foreach (var message in messages.OrderBy(message => message.Position))
            {
                bytes.AddRange(message.Data);
            }

            return bytes.ToArray();
        }
    }
}
