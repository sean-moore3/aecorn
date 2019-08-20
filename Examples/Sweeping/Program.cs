using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Aecorn.Sweeping;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var wlanSweep = new WlanSweep("VST2_01");
            wlanSweep.Run();
            wlanSweep.Close();
            //new SimpleFileSweep().Run();
            Console.WriteLine("Sweeping complete, press any key to exit");
            Console.ReadKey();
        }
    }
}
