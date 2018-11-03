namespace Nexus.Messages
{
    public interface IMessage
    {
        void Respounce<T>(T data);
        int Id { get; }
        object Data { get; set; }
        bool IsRequest { get; set; }
    }
}