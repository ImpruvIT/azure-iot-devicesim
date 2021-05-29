using System.Threading;
using System.Threading.Tasks;

namespace ImpruvIT.Azure.IoT.DeviceSimulator
{
    public interface IProvideServiceAsync<T>
    {
        Task<T> Provide(CancellationToken cancellationToken = default);
    }
}