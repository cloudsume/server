namespace Cloudsume.Resume
{
    using System;

    [Flags]
    public enum PropertyFlags : byte
    {
        None = 0x00,

        /// <summary>
        /// Do not fallback to the parent value if the value is <see langword="null"/>.
        /// </summary>
        Disabled = 0x01,
    }
}
