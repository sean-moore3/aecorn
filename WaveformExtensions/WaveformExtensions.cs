using System;
using System.Linq;

namespace NationalInstruments.Aecorn.WaveformExtensions
{
    public static class WaveformExtensions
    {
        #region Extension Methods for Static Functions
        public static void DecomposeArray(this ComplexSingle[] waveform, out float[] realData, out float[] imaginaryData)
        {
            ComplexSingle.DecomposeArray(waveform, out realData, out imaginaryData);
        }

        public static void DecomposeArray(this ComplexDouble[] waveform, out double[] realData, out double[] imaginaryData)
        {
            ComplexDouble.DecomposeArray(waveform, out realData, out imaginaryData);
        }

        public static void DecomposeRawDataArray(this ComplexWaveform<ComplexSingle> waveform, out float[] realData, out float[] imaginaryData)
        {
            var rawData = waveform.GetRawData();
            ComplexSingle.DecomposeArray(rawData, out realData, out imaginaryData);
        }

        public static void DecomposeRawDataArray(this ComplexWaveform<ComplexDouble> waveform, out double[] realData, out double[] imaginaryData)
        {
            var rawData = waveform.GetRawData();
            ComplexDouble.DecomposeArray(rawData, out realData, out imaginaryData);
        }

        public static float[] GetMagnitudes(this ComplexSingle[] waveform)
        {
            return ComplexSingle.GetMagnitudes(waveform);
        }

        public static double[] GetMagnitudes(this ComplexDouble[] waveform)
        {
            return ComplexDouble.GetMagnitudes(waveform);
        }

        public static float[] GetMagnitudeDataArrayFloat(this ComplexWaveform<ComplexSingle> waveform)
        {
            var rawData = waveform.GetRawData();
            return ComplexSingle.GetMagnitudes(rawData);
        }

        public static float[] GetPhases(this ComplexSingle[] waveform)
        {
            return ComplexSingle.GetPhases(waveform);
        }

        public static double[] GetPhases(this ComplexDouble[] waveform)
        {
            return ComplexDouble.GetPhases(waveform);
        }

        public static float[] GetPhaseDataArrayFloat(this ComplexWaveform<ComplexSingle> waveform)
        {
            var rawData = waveform.GetRawData();
            return ComplexSingle.GetPhases(rawData);
        }
        #endregion

        #region Duration
        public static double Duration(this ComplexWaveform<ComplexSingle> waveform)
        {
            return waveform.SampleCount * waveform.PrecisionTiming.SampleInterval.TotalSeconds;
        }

        public static double Duration(this ComplexWaveform<ComplexDouble> waveform)
        {
            return waveform.SampleCount * waveform.PrecisionTiming.SampleInterval.TotalSeconds;
        }
        #endregion

        #region Sample Rate
        public static double SampleRate(this ComplexWaveform<ComplexSingle> waveform)
        {
            return 1.0 / waveform.PrecisionTiming.SampleInterval.TotalSeconds;
        }

        public static double SampleRate(this ComplexWaveform<ComplexDouble> waveform)
        {
            return 1.0 / waveform.PrecisionTiming.SampleInterval.TotalSeconds;
        }
        #endregion

        #region Peak Power
        public static double PeakPower(this ComplexSingle[] waveform, bool simulated = false)
        {
            var maxMagSquared = waveform.Select((iq) => { return iq.Real * iq.Real + iq.Imaginary * iq.Imaginary; }).Max(); // prevents sqrt
            var peakPower = 10.0 * Math.Log10(maxMagSquared);
            return simulated ? peakPower : peakPower + 10.0;
        }

        public static double PeakPower(this ComplexDouble[] waveform, bool simulated = false)
        {
            var maxMagSquared = waveform.Select((iq) => { return iq.Real * iq.Real + iq.Imaginary * iq.Imaginary; }).Max(); // prevents sqrt
            var peakPower = 10.0 * Math.Log10(maxMagSquared);
            return simulated ? peakPower : peakPower + 10.0;
        }

        public static double PeakPower(this ComplexWaveform<ComplexSingle> waveform, bool simulated = false)
        {
            return waveform.GetRawData().PeakPower(simulated);
        }

        public static double PeakPower(this ComplexWaveform<ComplexDouble> waveform, bool simulated = false)
        {
            return waveform.GetRawData().PeakPower(simulated);
        }
        #endregion
    }
}