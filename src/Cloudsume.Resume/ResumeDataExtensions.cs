namespace Cloudsume.Resume;

using System;
using System.Linq.Expressions;
using Candidate.Server.Resume;

public static class ResumeDataExtensions
{
    public static string GetPropertyId<TData, TProp>(this TData data, Expression<Func<TData, DataProperty<TProp>>> expression) where TData : ResumeData
    {
        // Get property info.
        var property = expression.Body switch
        {
            MemberExpression e => e.Member switch
            {
                System.Reflection.PropertyInfo i => i,
                _ => throw new ArgumentException("The body must be a property.", nameof(expression)),
            },
            var body => throw new ArgumentException($"Expression '{body}' is not supported.", nameof(expression)),
        };

        // Get ID.
        return data.GetPropertyId(property);
    }
}
