namespace Cloudsume.Resume;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a property of the resume data.
/// </summary>
/// <typeparam name="T">
/// Type of the value, must be nullable type.
/// </typeparam>
public sealed class DataProperty<T> : IDataProperty
{
    public DataProperty(T? value, IDataSource source, PropertyFlags flags = PropertyFlags.None)
    {
        if (value is not null && flags.HasFlag(PropertyFlags.Disabled))
        {
            throw new ArgumentException($"The value contains {nameof(PropertyFlags.Disabled)} while {nameof(value)} is non-null.", nameof(flags));
        }

        this.Value = value;
        this.Source = source;
        this.Flags = flags;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => this.Value is not null;

    public bool IsFallback => this.Value is null && !this.Flags.HasFlag(PropertyFlags.Disabled);

    public PropertyFlags Flags { get; }

    public T? Value { get; }

    public IDataSource Source { get; }

    object? IDataProperty.Value => this.Value;

    public DataProperty<T> WithSource(IDataSource source) => new(this.Value, source, this.Flags);

    public DataProperty<T> WithValue(T? value, IDataSource source) => new(value, source, this.Flags);

    IDataProperty IDataProperty.WithValue(object? value, IDataSource source) => this.WithValue((T?)value, source);
}
