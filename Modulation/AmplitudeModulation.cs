using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NationalInstruments.Aecorn.Modulation
{
    public static class AmplitudeModulation
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
        public static IEnumerable<ComplexSingle> ModulateAM(this IEnumerable<float> waveform)
        {
            return Modulate(waveform);
        }

        public static IEnumerable<ComplexDouble> ModulateAM(this IEnumerable<double> waveform)
        {
            return Modulate(waveform);
        }

        public static IEnumerable<float> DemodulateAM(this IEnumerable<ComplexSingle> waveform)
        {
            return Demodulate(waveform);
        }

        public static IEnumerable<double> DemodulateAM(this IEnumerable<ComplexDouble> waveform)
        {
            return Demodulate(waveform);
        }
        #endregion
    }
}
