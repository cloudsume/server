namespace Cloudsume.Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Ultima.Extensions.DataValidation;

    public sealed class DeleteResumeData : IComparable<DeleteResumeData>, IComparable
    {
        public DeleteResumeData(string type, int index)
        {
            this.Type = type;
            this.Index = index;
        }

        [MaxLength(100)]
        public string Type { get; }

        [NonNegative]
        public int Index { get; }

        public int CompareTo(DeleteResumeData? other)
        {
            if (other == null)
            {
                return 1;
            }

            switch (this.Type.CompareTo(other.Type))
            {
                case > 0:
                    return 1;
                case < 0:
                    return -1;
            }

            return this.Index - other.Index;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj.GetType() != this.GetType())
            {
                throw new ArgumentException($"The value is not an instance of {this.GetType()}.", nameof(obj));
            }

            return this.CompareTo((DeleteResumeData)obj);
        }
    }
}
