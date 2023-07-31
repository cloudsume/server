namespace Cloudsume.Resume.Data
{
    using System;
    using Candidate.Server.Resume.Data;

    public sealed class TOEFL : LanguageProficiency
    {
        public TOEFL(int score)
        {
            if (score < 0 || score > 120)
            {
                throw new ArgumentOutOfRangeException(nameof(score));
            }

            this.Score = score;
        }

        public int Score { get; }
    }
}
