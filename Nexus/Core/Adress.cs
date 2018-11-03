namespace Nexus.Core
{
    public struct Adress
    {
        public string Data { get; }

        public Adress(string data)
        {
            Data = data;
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}