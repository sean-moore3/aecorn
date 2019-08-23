using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;

namespace Multithreading
{
    class SiteConfiguration
    {
        public readonly string resourceName;

        public RFmxInstrMX instr;
        public RFmxWlanMX wlan;

        public CommonConfiguration commonConfig = CommonConfiguration.GetDefault();
        public AutoLevelConfiguration autoLevelConfig = AutoLevelConfiguration.GetDefault();
        public SignalConfiguration signalConfig = SignalConfiguration.GetDefault();
        public OFDMModAccConfiguration ofdmModAccConfig = OFDMModAccConfiguration.GetDefault();

        public SiteConfiguration(string resourceName)
        {
            this.resourceName = resourceName;
        }
    }
}
