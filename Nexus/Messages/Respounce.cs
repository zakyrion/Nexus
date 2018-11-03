namespace Nexus.Messages
{
    public class Respounce
    {
        public int Id { get; }
        public dynamic Data { get; }

        public Respounce(int id, dynamic data)
        {
            Id = id;
            Data = data;
        }
    }
}