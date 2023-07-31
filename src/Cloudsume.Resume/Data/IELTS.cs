namespace Candidate.Server.Resume.Data
{
    using System;

    public sealed class IELTS : LanguageProficiency
    {
        public IELTS(TypeId type, decimal band)
        {
            if (band < 0 || band > 9 || (band % 0.5m) != 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(band));
            }

            this.Type = type;
            this.Band = band;
        }

        public enum TypeId
        {
            GeneralTraining = 0,
            Academic = 1,
        }

        public TypeId Type { get; }

        public decimal Band { get; }
    }
}
