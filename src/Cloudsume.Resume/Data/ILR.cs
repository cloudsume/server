namespace Candidate.Server.Resume.Data
{
    public sealed class ILR : LanguageProficiency
    {
        public ILR(ScaleId scale)
        {
            this.Scale = scale;
        }

        public enum ScaleId
        {
            NoProficiency = 0,
            Elementary = 1,
            LimitedWorking = 2,
            ProfessionalWorking = 3,
            FullProfessional = 4,
            Native = 5,
        }

        public ScaleId Scale { get; }
    }
}
