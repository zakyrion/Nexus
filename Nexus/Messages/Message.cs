using Nexus.Core.Channels;

namespace Nexus.Messages
{
    public class Message : IMessage
    {
        private readonly IChannel _from;
        public int Id { get; }
        public object Data { get; set; }

        public Message(int id, bool isRequest, object data, IChannel from = null)
        {
            Id = id;
            _from = from;
            Data = data;
            IsRequest = isRequest;
        }

        public bool IsRequest { get; set; }

        public void Respounce<T>(T data)
        {
            _from?.Send(data);
        }
    }
}