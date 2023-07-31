namespace Cloudsume.Resume;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public sealed class AssetName : IEquatable<AssetName>
{
    public static readonly int MaxLength = 200;

    private static readonly Rune Nul = new(0);
    private static readonly Rune Solidus = new('/');
    private static readonly BitArray ValidChars = GenerateValidChars();
    private static readonly IEnumerable<string> ReservedNames = new[]
    {
        "main.pdf",
        "output",
    };

    private readonly IReadOnlyList<string> parts;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetName"/> class.
    /// </summary>
    /// <param name="value">
    /// The value of this name.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is not a valid value.
    /// </exception>
    /// <remarks>
    /// Do not specify <paramref name="value"/> from a filesystem path due to it is not cross platform. Use <see cref="FromFileSystem(string)"/> instead.
    /// </remarks>
    public AssetName(string value)
    {
        var parts = new List<string>();

        this.parts = Parse<IReadOnlyList<string>>(value, parts, Array.Empty<string>(), part => parts.Add(part));

        if (this.parts.Count == 0)
        {
            throw new ArgumentException("The value is not a valid name.", nameof(value));
        }
    }

    /// <summary>
    /// Gets the parent of this asset.
    /// </summary>
    /// <value>
    /// Parts of the parent or empty if this asset has no parent.
    /// </value>
    public IEnumerable<string> Parent => this.parts.SkipLast(1);

    /// <summary>
    /// Gets a value of this name.
    /// </summary>
    /// <remarks>
    /// Don't use this value as a path on filesystem due to it is not cross platform. Use <see cref="ToFileSystem(string[])"/> instead.
    /// </remarks>
    public string Value => string.Join('/', this.parts);

    public static bool operator ==(AssetName a, string? b) => a.Value == b;

    public static bool operator ==(AssetName? a, AssetName? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        else if (a is null || b is null)
        {
            return false;
        }
        else
        {
            return a.Equals(b);
        }
    }

    public static bool operator !=(AssetName a, string? b) => !(a == b);

    public static bool operator !=(AssetName? a, AssetName? b) => !(a == b);

    public static bool IsValid(string value) => Parse(value, true, false, null);

    /// <summary>
    /// Create a <see cref="AssetName"/> from a filesystem path.
    /// </summary>
    /// <param name="path">
    /// Path of the asset, in relative form.
    /// </param>
    /// <returns>
    /// Asset name of the specified path.
    /// </returns>
    public static AssetName FromFileSystem(string path)
    {
        return Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => new(path),
            _ => throw new NotImplementedException(),
        };
    }

    public bool Equals(AssetName? other)
    {
        if (other is null || other.parts.Count != this.parts.Count)
        {
            return false;
        }

        for (var i = 0; i < other.parts.Count; i++)
        {
            if (other.parts[i] != this.parts[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is AssetName other && this.Equals(other);

    public override int GetHashCode() => this.parts.Aggregate(0, (previous, current) => (previous << 1) ^ current.GetHashCode());

    public string ToFileSystem(params string[] prefixes) => Path.Join(prefixes.Concat(this.parts).ToArray());

    private static BitArray GenerateValidChars()
    {
        var allows = new BitArray(128); // Maximum ASCII.

        foreach (var ch in " -.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz")
        {
            allows.Set(ch, true);
        }

        return allows;
    }

    private static T Parse<T>(string value, T valid, T invalid, Action<string>? process)
    {
        // Sanity check.
        if (value.Length == 0 || value.Length > MaxLength || value[0] == '/' || value.Last() == '/' || value.StartsWith("~/", StringComparison.Ordinal))
        {
            return invalid;
        }

        foreach (var reserved in ReservedNames)
        {
            if (value == reserved || value.StartsWith($"{reserved}/", StringComparison.Ordinal))
            {
                return invalid;
            }
        }

        // Parse.
        var start = 0;
        var i = 0;
        var parts = 0;

        while (i < value.Length)
        {
            if (!Rune.TryGetRuneAt(value, i, out var r))
            {
                return invalid;
            }
            else if (r == Solidus)
            {
                if (!Flush())
                {
                    return invalid;
                }

                start = i + r.Utf16SequenceLength;
                parts++;
            }
            else if (r == Nul)
            {
                return invalid;
            }

            i += r.Utf16SequenceLength;
        }

        // Parse last part.
        if (!Flush())
        {
            return invalid;
        }

        parts++;

        if (parts > 10)
        {
            return invalid;
        }

        return valid;

        bool Flush()
        {
            var part = value[start..i];

            if (part.Length == 0 || char.IsWhiteSpace(part[0]) || char.IsWhiteSpace(part.Last()) || part == "." || part == "..")
            {
                return false;
            }

            foreach (var ch in part)
            {
                if (ch >= ValidChars.Count || !ValidChars.Get(ch))
                {
                    return false;
                }
            }

            process?.Invoke(part);

            return true;
        }
    }
}
