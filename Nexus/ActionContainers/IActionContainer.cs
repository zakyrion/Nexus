namespace Nexus.ActionContainers
{
    public interface IActionContainer
    {
        void Invoke(object param = null);
    }
}