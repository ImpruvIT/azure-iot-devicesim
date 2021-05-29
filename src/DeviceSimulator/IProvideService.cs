namespace ImpruvIT.Azure.IoT.DeviceSimulator
{
    public interface IProvideService<T>
    {
        T Provide();
    }
}