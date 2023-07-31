namespace Candidate.Server.Resume.Data
{
    using System;

    public sealed class TOEIC : LanguageProficiency
    {
        public TOEIC(int score)
        {
            if (score < 0 || score > 990)
            {
                throw new ArgumentOutOfRangeException(nameof(score));
            }

            this.Score = score;
        }

        public int Score { get; }
    }
}
