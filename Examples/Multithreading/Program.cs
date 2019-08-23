using System;

namespace Multithreading
{
    class Program
    {
        static void Main(string[] args)
        {
            var wlan = new Wlan();
            wlan.Run();
            Console.WriteLine("Program complete. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
