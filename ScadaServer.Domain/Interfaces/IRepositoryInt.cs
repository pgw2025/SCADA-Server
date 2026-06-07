using ScadaServer.Domain.Interfaces;

namespace ScadaServer.Domain.Interfaces
{
    /// <summary>
    /// Base repository interface for entities with int keys.
    /// </summary>
    public interface IRepository<T> : IRepository<T, int> where T : class, new()
    {
    }
}
