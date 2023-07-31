namespace Candidate.Server.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class GlobalDataComparer : EqualityComparer<ResumeData>
    {
        public static new GlobalDataComparer Default { get; } = new();

        public override bool Equals(ResumeData? x, ResumeData? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Type != y.Type)
            {
                return false;
            }

            if (x is MultiplicativeData left)
            {
                var right = (MultiplicativeData)y;

                if (left.Id == null || left.Id.Value == Guid.Empty)
                {
                    throw new ArgumentException("Aggregated or local data is not supported.", nameof(x));
                }

                if (right.Id == null || right.Id.Value == Guid.Empty)
                {
                    throw new ArgumentException("Aggregated or local data is not supported.", nameof(y));
                }

                if (left.Id.Value != right.Id.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode([DisallowNull] ResumeData obj)
        {
            var result = obj.Type.GetHashCode();

            if (obj is MultiplicativeData m)
            {
                if (m.Id == null || m.Id.Value == Guid.Empty)
                {
                    throw new ArgumentException("Aggregated or local data is not supported.", nameof(obj));
                }

                result ^= m.Id.Value.GetHashCode();
            }

            return result;
        }
    }
}
