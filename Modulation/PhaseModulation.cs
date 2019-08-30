using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NationalInstruments.Aecorn.Modulation
{
    public static class PhaseModulation
    {
        #region Static Methods
        public static IEnumerable<ComplexSingle> Modulate(IEnumerable<float> waveform)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<ComplexDouble> Modulate(IEnumerable<double> waveform)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<float> Demodulate(IEnumerable<ComplexSingle> waveform)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<double> Demodulate(IEnumerable<ComplexDouble> waveform)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Extension Methods
        public static IEnumerable<ComplexSingle> ModulatePM(this IEnumerable<float> waveform)
        {
            return Modulate(waveform);
        }

        public static IEnumerable<ComplexDouble> ModulatePM(this IEnumerable<double> waveform)
        {
            return Modulate(waveform);
        }

        public static IEnumerable<float> DemodulatePM(this IEnumerable<ComplexSingle> waveform)
        {
            return Demodulate(waveform);
        }

        public static IEnumerable<double> DemodulatePM(this IEnumerable<ComplexDouble> waveform)
        {
            return Demodulate(waveform);
        }
        #endregion
    }
}
