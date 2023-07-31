namespace Candidate.Server.Resume
{
    using System;

    public sealed class MultiplicativeLocalDataUpdate : LocalDataUpdate
    {
        public MultiplicativeLocalDataUpdate(MultiplicativeData value, int index)
            : base(value)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.Index = index;
        }

        public new MultiplicativeData Value => (MultiplicativeData)base.Value;

        public int Index { get; }
    }
}
