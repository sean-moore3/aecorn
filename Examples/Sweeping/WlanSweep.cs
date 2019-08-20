using NationalInstruments.Aecorn.Sweeping;
using NationalInstruments.ModularInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Examples
{
    class WlanSweep
    {
        private NIRfsg rfsgSession;
        private IntPtr rfsgHandle;

        private RFmxInstrMX instr;
        private RFmxWlanMX wlan;

        private niPowerServo powerServo;

        private readonly IEnumerable<double> frequencySweepPoints = Enumerable.Repeat(5.18e9, 1);
        private readonly IEnumerable<int> averagesSweepPoints = Enumerable.Range(1, 20);
        private readonly IEnumerable<int> symbolsSweepPoints = Enumerable.Range(1, 16);

        private List<ISweepStep> sweepSteps = new List<ISweepStep>();

        private readonly int repeatabilityCount = 100;

        // Generation
        public string WaveformPath = "80211ac_80M_MSC9.tdms";

        // DUT Params
        public double DutDesiredOutputPower = -10.0;
        public double DutEstimatedGain = 0;
        public double DutGainAccuracy = 2.0;

        // Servo Params
        public double ServoAccuracy = 0.05;
        public ushort ServoMaxNumberOfSteps = 10;
        public ushort ServoMinNumberOfSteps = 2;
        public double ServoInitialAveragingTime = 100e-6;
        public double ServoFinalAveragingTime = 500e-6;
        public double ServoTimeout = 10.0;

        public WlanSweep(string resourceName)
        {
            rfsgSession = new NIRfsg(resourceName, false, false, "DriverSetup=Bitfile:NI-RFIC.lvbitx");
            rfsgHandle = rfsgSession.GetInstrumentHandle().DangerousGetHandle();

            instr = new RFmxInstrMX(resourceName, "RFmxSetup=Bitfile:NI-RFIC.lvbitx");
            wlan = instr.GetWlanSignalConfiguration();

            instr.DangerousGetNIRfsaHandle(out IntPtr rfsaHandle);
            powerServo = new niPowerServo(rfsaHandle, false);
        }

        public void Run()
        {
            ConfigureGeneration();
            ConfigureMeasurement();
            ConfigureServo();
            ConfigureSweep();

            rfsgSession.Initiate();

            // Define sweep
            Stopwatch watch = new Stopwatch();
            watch.Reset();

            StreamWriter stream = new StreamWriter(File.Create("data.csv"));
            stream.WriteLine(string.Join(", ", "Frequency", "Averages", "Symbols"));

            var sweep = new Sweep(sweepSteps, () =>
            {
                // get rid of first run jitter
                wlan.Initiate("", "");
                wlan.WaitForMeasurementComplete("", 10.0);
                wlan.OfdmModAcc.Configuration.GetAveragingCount("", out int numAverages);
                wlan.OfdmModAcc.Results.GetNumberOfSymbolsUsed("", out int numSymbolsUsed);
                wlan.OfdmModAcc.Results.GetCompositeRmsEvmMean("", out double evm);

                wlan.GetCenterFrequency("", out double centFreq);
                string currentPoints = string.Join(", ", centFreq.ToString(), sweepSteps[1].GetCurrentPoint().ToString(), 
                    sweepSteps[2].GetCurrentPoint().ToString());

                stream.Write(currentPoints);
                Console.WriteLine(currentPoints);

                for (int i = 0; i < repeatabilityCount; i++)
                {
                    powerServo.Disable();
                    powerServo.Enable();

                    watch.Restart();

                    wlan.Initiate("", "");
                    
                    powerServo.Start();
                    powerServo.Wait(ServoTimeout, out ushort servoNumberOfAveragesCaptured, out bool servoDone, out bool servoFailed);
                    // powerServo.GetDigitalGain(out double servoRawDigitalGain, out double servoDigitalGain);
                    // powerServo.GetServoSteps(servoNumberOfAveragesCaptured, servoContinuous, false, 0, out servoMeasurementTimes, out servoMeasurementLevels);
                    
                    wlan.WaitForMeasurementComplete("", 10.0);
                    wlan.OfdmModAcc.Configuration.GetAveragingCount("", out numAverages);
                    wlan.OfdmModAcc.Results.GetCompositeRmsEvmMean("", out evm);
                    wlan.OfdmModAcc.Results.GetNumberOfSymbolsUsed("", out numSymbolsUsed);

                    watch.Stop();

                    if (numAverages != (int)sweepSteps[1].GetCurrentPoint() || numSymbolsUsed != (int)sweepSteps[2].GetCurrentPoint())
                        throw new Exception();

                    stream.Write(", " + evm.ToString());
                    stream.Write(", " + watch.ElapsedMilliseconds.ToString());
                }

                stream.Write(stream.NewLine);
            });

            sweep.Run();
            rfsgSession.Abort();
            stream.Close();
        }

        private void ConfigureGeneration()
        {
            rfsgSession.Arb.GenerationMode = RfsgWaveformGenerationMode.Script;
            NIRfsgPlayback.ReadAndDownloadWaveformFromFile(rfsgHandle, WaveformPath, "wlanwfm");
            NIRfsgPlayback.StoreAutomaticSGSASharedLO(rfsgHandle, "", RfsgPlaybackAutomaticSGSASharedLO.Enabled);
            string script = "script myScript repeat forever generate wlanwfm end repeat end script";
            NIRfsgPlayback.SetScriptToGenerateSingleRfsg(rfsgHandle, script);
        }

        private void ConfigureMeasurement()
        {
            wlan.SelectMeasurements("", RFmxWlanMXMeasurementTypes.OfdmModAcc, false);
            wlan.ConfigureStandard("", RFmxWlanMXStandard.Standard802_11ac);
            wlan.ConfigureChannelBandwidth("", 80e6);
            wlan.ConfigureReferenceLevel("", 0.0);
            wlan.ConfigureIQPowerEdgeTrigger("", "0", RFmxWlanMXIQPowerEdgeTriggerSlope.Rising, -20, 0,
                RFmxWlanMXTriggerMinimumQuietTimeMode.Auto, 0, RFmxWlanMXIQPowerEdgeTriggerLevelType.Relative, true);
            wlan.SetOfdmAutoPpduTypeDetectionEnabled("", RFmxWlanMXOfdmAutoPpduTypeDetectionEnabled.False);
            wlan.SetOfdmHeaderDecodingEnabled("", RFmxWlanMXOfdmHeaderDecodingEnabled.False);
            wlan.SetOfdmMcsIndex("", 9);
            wlan.SetOfdmPpduType("", RFmxWlanMXOfdmPpduType.SU);
            instr.SetAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);
        }

        private void ConfigureServo()
        {
            wlan.Commit("");

            powerServo.ConfigureVSAReferenceTriggerOverride(ReferenceTriggerOverride.Arm_Ref_Trig_On_Servo_Done);
            powerServo.DigitalGainStepLimitEnabled(false);
            NIRfsgPlayback.ReadPaprFromFile(WaveformPath, 0, out double papr);
            powerServo.CalculateServoParams(DutDesiredOutputPower, DutEstimatedGain, DutGainAccuracy, papr, out double rfAnalyzerActualReferenceLevel, out double rfGeneratorActualAveragePower);

            wlan.SetReferenceLevel("", rfAnalyzerActualReferenceLevel);
            rfsgSession.RF.PowerLevel = rfGeneratorActualAveragePower;

            powerServo.Setup(ServoAccuracy, ServoInitialAveragingTime, ServoFinalAveragingTime, ServoMinNumberOfSteps, ServoMaxNumberOfSteps, false, 0);
            int[] burstStartLocations = null, burstStopLocations = null;
            NIRfsgPlayback.ReadBurstStartLocationsFromFile(WaveformPath, 0, ref burstStartLocations);
            NIRfsgPlayback.ReadBurstStopLocationsFromFile(WaveformPath, 0, ref burstStopLocations);
            NIRfsgPlayback.ReadSampleRateFromFile(WaveformPath, 0, out double iqRate);
            powerServo.ConfigureActiveServoWindow(true, (burstStopLocations.First() - burstStartLocations.First()) / iqRate);
        }

        private void ConfigureSweep()
        {
            var freqSweepStep = new SweepStep<double>(frequencySweepPoints, (freq) =>
            {
                wlan.ConfigureFrequency("", freq);
                rfsgSession.RF.Frequency = freq;
            });

            var averagesSweepStep = new SweepStep<int>(averagesSweepPoints, (numAverages) =>
            {
                wlan.OfdmModAcc.Configuration.ConfigureAveraging("", RFmxWlanMXOfdmModAccAveragingEnabled.True, numAverages);
            });

            var symbolsSweepStep = new SweepStep<int>(symbolsSweepPoints, (numSymbols) =>
            {
                wlan.OfdmModAcc.Configuration.ConfigureMeasurementLength("", 0, numSymbols);
            });

            sweepSteps = new List<ISweepStep>
            {
                freqSweepStep,
                averagesSweepStep,
                symbolsSweepStep
            };
        }

        public void Close()
        {
            powerServo.Close();
            rfsgSession.Close();
            instr.Close();
        }
    }
}
