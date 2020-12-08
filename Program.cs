using System;
using System.Threading.Tasks;

namespace PhoenixLeds
{
    class Program
    {
        private static async Task Main(string[] args) {
            await GlobalSettings.Load();

            Console.WriteLine("Hello World!");
        }
    }
}
