using NationalInstruments.Aecorn.Threading;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System.IO;
using System.Collections.Generic;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;

namespace Multithreading
{
    class Wlan
    {
        // Resources
        private string[] resourceNames = new string[] { "VST1_01", "VST1_02" };

        // Derived Attributes
        private int NumberOfSites { get { return resourceNames.Length; } }

        // Sites
        private SiteConfiguration[] sites;
        private ConsumerThread[] threads;

        // Result Logging
        private StreamWriter[] streams;
        private ConsumerThread loggingThread;

        public void Run()
        {
            // create sites, threads, and streams arrays
            sites = new SiteConfiguration[NumberOfSites];
            threads = new ConsumerThread[NumberOfSites];
            streams = new StreamWriter[NumberOfSites];

            for (int i = 0; i < NumberOfSites; i++)
            {
                sites[i] = new SiteConfiguration(resourceNames[i]); // create site configuration
                threads[i] = new ConsumerThread(); // spawn consumer thread
                threads[i].Enqueue(Callback.New(InitializeSessions, sites[i])); // queue instrument initialization

                // set configuration data in each site
                sites[i].commonConfig.CenterFrequency_Hz = 5.18e9;
                sites[i].commonConfig.FrequencyReferenceSource = RFmxInstrMXConstants.OnboardClock;
                sites[i].commonConfig.LOSource = RFmxInstrMXConstants.LOSourceOnboard;
                sites[i].commonConfig.DigitalEdgeSource = "PXI_Trig" + i;
                sites[i].signalConfig.AutoDetectSignal = false;
                sites[i].signalConfig.ChannelBandwidth_Hz = 80e6;
                sites[i].signalConfig.Standard = RFmxWlanMXStandard.Standard802_11ac;

                streams[i] = new StreamWriter(File.Create(sites[i].resourceName + "_results.csv")); // create file for logging to disk
            }

            // spawn thread for logging to disk
            loggingThread = new ConsumerThread();

            // wait for sessions to initialize before continuing
            for (int i = 0; i < NumberOfSites; i++)
                threads[i].Wait();

            // queue work items into threads
            for (int i = 0; i < NumberOfSites; i++)
            {
                // create callback list
                var callbackList = new List<ICallable>
                {
                    // RDL configurations
                    Callback.New(ConfigureCommon, sites[i].instr, sites[i].wlan, sites[i].commonConfig, sites[i].autoLevelConfig, ""),
                    Callback.New(PowerEdgeTriggerOverride, sites[i]), // override digital edge trigger with power edge trigger
                    Callback.New(ConfigureSignal, sites[i].wlan, sites[i].signalConfig, ""),
                    Callback.New(ConfigureOFDMModAcc, sites[i].wlan, sites[i].ofdmModAccConfig, ""),

                    // initate and measure
                    Callback.New((site) => site.wlan.Initiate("", ""), sites[i]),
                    Callback.New(Measure, sites[i], streams[i])
                };

                // enqueue callbacks
                foreach (ICallable callback in callbackList)
                    threads[i].Enqueue(callback);
            }
          
            // close instruments
            for (int i = 0; i < NumberOfSites; i++)
                threads[i].Enqueue(Callback.New(sites[i].instr.Close));

            // close threads
            foreach (var thread in threads)
                thread.Finish();

            // close file streams
            foreach (var fileStream in streams)
                loggingThread.Enqueue(Callback.New(fileStream.Close));

            // let logging thread finish working
            loggingThread.Finish();
        }

        private void InitializeSessions(SiteConfiguration site)
        {
            site.instr = new RFmxInstrMX(site.resourceName, "");
            site.wlan = site.instr.GetWlanSignalConfiguration();
        }

        private void PowerEdgeTriggerOverride(SiteConfiguration site)
        {
            site.wlan.ConfigureIQPowerEdgeTrigger("", "0", RFmxWlanMXIQPowerEdgeTriggerSlope.Rising, -20.0, 0.0, 
                RFmxWlanMXTriggerMinimumQuietTimeMode.Auto, 0.0, RFmxWlanMXIQPowerEdgeTriggerLevelType.Relative, true);
        }

        private void Measure(SiteConfiguration site, StreamWriter fileStream)
        {
            OFDMModAccResults results = FetchOFDMModAcc(site.wlan);
            loggingThread.Enqueue(Callback.New((stream, rst) => stream.WriteLine(rst.CompositeRMSEVMMean_dB), fileStream, results));
        }
    }
}
