namespace Candidate.Server.Resume
{
    using System.Collections.Generic;

    public static class ResumeComparer
    {
        public static readonly EqualityComparer<ResumeInfo> ByTemplate = new TemplateComparer();

        private sealed class TemplateComparer : EqualityComparer<ResumeInfo>
        {
            public override bool Equals(ResumeInfo? x, ResumeInfo? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                else if (x == null || y == null)
                {
                    return false;
                }

                return x.TemplateId == y.TemplateId;
            }

            public override int GetHashCode(ResumeInfo obj) => obj.TemplateId.GetHashCode();
        }
    }
}
