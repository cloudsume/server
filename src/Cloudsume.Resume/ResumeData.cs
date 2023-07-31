namespace Candidate.Server.Resume
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cloudsume.Resume;
    using PropertyInfo = Cloudsume.Resume.PropertyInfo;

    public abstract class ResumeData
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> Properties = new();

        protected ResumeData(DateTime updatedAt)
        {
            this.UpdatedAt = updatedAt;
        }

        public abstract string Type { get; }

        public bool HasFallbacks => this.GetPropertyValues().Any(p => p.IsFallback);

        public bool HasValue => this.GetPropertyValues().Any(p => p.HasValue);

        public DateTime UpdatedAt { get; }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="id">
        /// The identifier of the property (e.g. <see cref="Data.Name.FirstNameProperty"/>).
        /// </param>
        /// <returns>
        /// The value of the property or <see langword="null"/> if <paramref name="id"/> is not a valid property identifier.
        /// </returns>
        public IDataProperty? GetPropertyValue(string id)
        {
            var property = this.GetProperties().FirstOrDefault(p => p.Id == id);

            return property?.GetValue(this);
        }

        /// <summary>
        /// Gets the identifier of the specified property.
        /// </summary>
        /// <param name="property">
        /// The property to get the identifier.
        /// </param>
        /// <returns>
        /// The identifier of the property (e.g. <see cref="Data.Name.FirstNameProperty"/>).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not represent a property of this data.
        /// </exception>
        /// <remarks>
        /// Use <see cref="ResumeDataExtensions.GetPropertyId{TData, TProp}(TData, System.Linq.Expressions.Expression{Func{TData, DataProperty{TProp}}})"/>
        /// instead if you don't have <see cref="System.Reflection.PropertyInfo"/>.
        /// </remarks>
        public string GetPropertyId(System.Reflection.PropertyInfo property)
        {
            var info = this.GetProperties().FirstOrDefault(p => p.Equals(property));

            if (info is null)
            {
                throw new ArgumentException($"The value is not a property of {this.GetType()}.", nameof(property));
            }

            return info.Id;
        }

        private IEnumerable<IDataProperty> GetPropertyValues()
        {
            foreach (var property in this.GetProperties())
            {
                yield return property.GetValue(this);
            }
        }

        private IEnumerable<PropertyInfo> GetProperties() => Properties.GetOrAdd(this.GetType(), type =>
        {
            var properties = new List<PropertyInfo>();

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // Check if the propert is a data property.
                var attribute = property.GetCustomAttribute<DataPropertyAttribute>();

                if (attribute is null)
                {
                    continue;
                }

                properties.Add(new(type, property, attribute));
            }

            return properties;
        });
    }
}
