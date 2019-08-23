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
        private readonly string[] resourceNames = new string[] { "VST1_01", "VST1_02" };

        // Derived Attributes
        private int NumberOfSites { get { return resourceNames.Length; } }

        // Sites
        private readonly SiteConfiguration[] sites;
        private readonly ConsumerThread[] consumerThreads;

        // Result Logging
        private readonly StreamWriter[] streams;
        private readonly ConsumerThread loggingThread;

        public Wlan()
        {
            // create individual sites
            sites = new SiteConfiguration[NumberOfSites];
            for (int i = 0; i < NumberOfSites; i++)
                sites[i] = new SiteConfiguration(resourceNames[i]);

            // spawn new consumer threads
            consumerThreads = new ConsumerThread[NumberOfSites];
            for (int i = 0; i < NumberOfSites; i++)
                consumerThreads[i] = new ConsumerThread();

            // spawn thread for logging to disk
            loggingThread = new ConsumerThread();

            // command each thread to initialize the instrument
            for (int i = 0; i < NumberOfSites; i++)
                consumerThreads[i].Enqueue(Callback.New(InitializeSessions, sites[i]));

            // configure constants on each site
            for (int i = 0; i < NumberOfSites; i++)
            {
                sites[i].commonConfig.CenterFrequency_Hz = 5.18e9;
                sites[i].commonConfig.FrequencyReferenceSource = RFmxInstrMXConstants.OnboardClock;
                sites[i].commonConfig.LOSource = RFmxInstrMXConstants.LOSourceOnboard;
                sites[i].commonConfig.DigitalEdgeSource = "PXI_Trig" + i;
                sites[i].signalConfig.AutoDetectSignal = false;
                sites[i].signalConfig.Standard = RFmxWlanMXStandard.Standard802_11ac;
                sites[i].signalConfig.ChannelBandwidth_Hz = 80e6;
            }

            // create streams for writing results to file
            streams = new StreamWriter[NumberOfSites];
        }

        private void InitializeSessions(SiteConfiguration site)
        {
            site.instr = new RFmxInstrMX(site.resourceName, "");
            site.wlan = site.instr.GetWlanSignalConfiguration();
        }

        private void SharedLOOverride(RFmxInstrMX instr)
        {
            // override settings from rdl to implement auto shared lo
            instr.ResetAttribute("", RFmxInstrMXPropertyId.LOSource);
            instr.ResetAttribute("", RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
            instr.ConfigureAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);
        }

        private void Measure(SiteConfiguration site, StreamWriter fileStream)
        {
            OFDMModAccResults results = FetchOFDMModAcc(site.wlan);
            loggingThread.Enqueue(Callback.New((stream, rst) => stream.WriteLine(rst.CompositeRMSEVMMean_dB), fileStream, results));
        }

        public void Run()
        {
            // create files for logging to disk
            for (int i = 0; i < NumberOfSites; i++)
                streams[i] = new StreamWriter(File.Create(sites[i].resourceName + "_results.csv"));

            // wait for sessions to initialize before continuing
            for (int i = 0; i < NumberOfSites; i++)
                consumerThreads[i].Wait();

            // kick off the threads
            for (int i = 0; i < NumberOfSites; i++)
            {
                // create callback list
                var callbackList = new List<ICallable>
                {
                    Callback.New(ConfigureCommon, sites[i].instr, sites[i].wlan, sites[i].commonConfig, sites[i].autoLevelConfig, ""),
                    Callback.New(ConfigureSignal, sites[i].wlan, sites[i].signalConfig, ""),
                    Callback.New(ConfigureOFDMModAcc, sites[i].wlan, sites[i].ofdmModAccConfig, ""),
                    Callback.New((site) => site.wlan.Initiate("", ""), sites[i]),
                    Callback.New(Measure, sites[i], streams[i])
                };

                // enqueue callbacks
                foreach (ICallable callback in callbackList)
                    consumerThreads[i].Enqueue(callback);
            }

            // wait for everything to finish running
            foreach (var thread in consumerThreads)
                thread.Wait();
           
            // close instruments
            for (int i = 0; i < NumberOfSites; i++)
                consumerThreads[i].Enqueue(Callback.New(sites[i].instr.Close));

            // close threads
            foreach (var thread in consumerThreads)
                thread.Join();

            // close file streams
            foreach (var fileStream in streams)
                loggingThread.Enqueue(Callback.New(fileStream.Close));

            // let logging thread finish working
            loggingThread.Join();
        }
    }
}
