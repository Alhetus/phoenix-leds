using System.Threading.Tasks;

namespace PhoenixLeds
{
    class Program
    {
        private static async Task Main(string[] args) {
            // Load settings
            await GlobalSettings.LoadAsync();

            // Load animation events
            await AnimationEventModel.LoadAsync();

            // Open serial port
            SerialCommunicator.InitSerialConnection();

            // Load animations from .anim files
            var ledAnimationController = new LedAnimationController();
            await ledAnimationController.LoadAnimationsAsync();

            // Do not exit the application
            while (true) {
                await Task.Delay(100000);
            }
        }
    }
}
