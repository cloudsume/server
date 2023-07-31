namespace Cloudsume.Resume;

using System;
using System.Linq.Expressions;
using System.Reflection;
using Candidate.Server.Resume;

internal sealed class PropertyInfo : IEquatable<System.Reflection.PropertyInfo>
{
    private readonly System.Reflection.PropertyInfo data;
    private readonly Func<ResumeData, IDataProperty> getter;

    public PropertyInfo(Type data, System.Reflection.PropertyInfo property, DataPropertyAttribute attribute)
    {
        var getter = property.GetGetMethod();

        if (getter is null)
        {
            throw new ArgumentException($"Property does not have a public getter.", nameof(property));
        }

        this.data = property;
        this.getter = BuildGetter(data, getter);
        this.Id = attribute.Id;
    }

    public string Id { get; }

    public bool Equals(System.Reflection.PropertyInfo? other) => this.data == other;

    public IDataProperty GetValue(ResumeData data) => this.getter.Invoke(data);

    private static Func<ResumeData, IDataProperty> BuildGetter(Type data, MethodInfo method)
    {
        var arg = Expression.Parameter(typeof(ResumeData));
        var body = Expression.Convert(Expression.Call(Expression.Convert(arg, data), method), typeof(IDataProperty));
        var getter = Expression.Lambda<Func<ResumeData, IDataProperty>>(body, arg);

        return getter.Compile();
    }
}
