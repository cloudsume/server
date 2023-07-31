namespace Cloudsume.Resume;

/// <summary>
/// Represents a property of resume data.
/// </summary>
public interface IDataProperty
{
    bool HasValue { get; }

    bool IsFallback { get; }

    PropertyFlags Flags { get; }

    object? Value { get; }

    IDataSource Source { get; }

    IDataProperty WithValue(object? value, IDataSource source);
}
