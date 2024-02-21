using ServiceWire.TcpIp;
using Windows.Gaming.Input;
using XboxWheelCompatibility.CommunicationInterface;
using XboxWheelCompatibility.WheelTransformer;

namespace XboxWheelCompatibility.WheelCompatibilityService
{
    public class WheelCompatibilityWorker : BackgroundService, IWheelCompatibilityService
    {
        private readonly ILogger<WheelCompatibilityWorker> Logger;
        private readonly TcpHost TCPHost;

        public int GetMainWheelIndex()
{
    var racingWheels = WheelManager.ActiveWheels;

    // Iterate through each racing wheel
    for (int i = 0; i < racingWheels.Count; i++)
    {
        // Check if the racing wheel is the Ferrari 458 Spider wheel
        if (WheelManager.IsFerrari458SpiderWheel(racingWheels[i]))
        {
            // Found the Ferrari 458 Spider wheel, return its index
            return i;
        }
    }

    // If the Ferrari 458 Spider wheel is not found, return -1 or handle accordingly
    return -1;
}
        public void Start()
        {
            WheelInputTransformer.Start();
        }

        public void Stop()
        {
            WheelInputTransformer.Stop();
        }

        public WheelCompatibilityWorker(ILogger<WheelCompatibilityWorker> logger)
        {
            Logger = logger;
            TCPHost = new TcpHost(16581);
        }

        protected override Task ExecuteAsync(CancellationToken Cancellation)
        {
            TCPHost.AddService<IWheelCompatibilityService>(this);

            WheelInputTransformer.Start();

            TCPHost.Open();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken Cancellation)
        {
            TCPHost.Close();

            WheelInputTransformer.Stop();

            return Task.CompletedTask;
        }
    }
}
