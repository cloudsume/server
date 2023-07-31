namespace Cornot
{
    using System;

    /// <summary>
    /// Represents errors that occur when application trying to access corrupted data.
    /// </summary>
    public sealed class DataCorruptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCorruptionException"/> class.
        /// </summary>
        /// <param name="data">
        /// The corrupted data.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the corruption.
        /// </param>
        public DataCorruptionException(object data, string message)
            : this(data, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCorruptionException"/> class.
        /// </summary>
        /// <param name="data">
        /// The corrupted data.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the corruption.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <c>null</c> if no inner exception is specified.
        /// </param>
        public DataCorruptionException(object data, string message, Exception? innerException)
            : base(message, innerException)
        {
            this.CorruptedData = data;
        }

        public object CorruptedData { get; }
    }
}
