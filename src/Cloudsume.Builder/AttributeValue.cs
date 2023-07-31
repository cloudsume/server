namespace Candidate.Server.Resume.Builder
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class AttributeValue<TType, TValue>
        where TType : class
    {
        private static readonly ConstructorInfo TargetConstructor = typeof(TType).GetConstructors().Single(IsTarget);
        private static readonly Func<TValue, TType> Factory = CreateFactory(TargetConstructor);

        protected AttributeValue(TValue value)
        {
            this.Value = value;
        }

        public TValue Value { get; }

        public static TType? From(TValue? value)
        {
            return value == null ? null : Factory(value);
        }

        private static bool IsTarget(ConstructorInfo info)
        {
            var parameters = info.GetParameters();

            if (parameters.Length != 1)
            {
                return false;
            }

            return parameters[0].ParameterType == typeof(TValue);
        }

        private static Func<TValue, TType> CreateFactory(ConstructorInfo constructor)
        {
            var arg = Expression.Variable(typeof(TValue));
            var body = Expression.New(constructor, arg);
            var lambda = Expression.Lambda<Func<TValue, TType>>(body, arg);

            return lambda.Compile();
        }
    }
}
