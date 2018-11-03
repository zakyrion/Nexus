namespace Nexus.Messages
{
    public class Request
    {
        public int Id { get; }
        public dynamic Data { get; }

        public Request(int id, dynamic data)
        {
            Id = id;
            Data = data;
        }
    }
}