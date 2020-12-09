using System.Threading.Tasks;

namespace PhoenixLeds
{
    class Program
    {
        private static async Task Main(string[] args) {
            // Load settings
            await GlobalSettings.LoadAsync();

            // Open serial port
            SerialCommunicator.InitSerialConnection();

            // Load animations from .anim files
            await LedAnimationLoader.LoadAnimationsAsync();

            // Do not exit the application
            while (true) { }
        }
    }
}
