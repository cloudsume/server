namespace Cloudsume.Resume
{
    /// <summary>
    /// Category of the template.
    /// </summary>
    /// <remarks>
    /// Category change restrictions:
    /// <para>
    /// <see cref="RegistrationCategory.Free"/> and <see cref="RegistrationCategory.Paid"/> cannot change to <see cref="RegistrationCategory.Private"/> due to
    /// it will break current users from getting registration info.
    /// </para>
    /// <para>
    /// </remarks>
    public enum RegistrationCategory : sbyte
    {
        Free = 0,
        Private = 1,
        Paid = 2,
    }
}
