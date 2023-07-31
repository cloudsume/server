namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cloudsume.Cassandra.Models;
using global::Cassandra.Mapping;

internal sealed class SampleDataManager
{
    private readonly Func<IResumeSampleData> newDto;
    private readonly Func<IMapper, Cql, Task<IEnumerable<IResumeSampleData>>> fetch;
    private readonly Func<IMapper, IResumeSampleData, CqlQueryOptions, Task> update;
    private readonly Func<IMapper, IResumeSampleData, CqlQueryOptions, Task> delete;

    public SampleDataManager(
        Func<IResumeSampleData> newDto,
        Func<IMapper, Cql, Task<IEnumerable<IResumeSampleData>>> fetch,
        Func<IMapper, IResumeSampleData, CqlQueryOptions, Task> update,
        Func<IMapper, IResumeSampleData, CqlQueryOptions, Task> delete)
    {
        this.newDto = newDto;
        this.fetch = fetch;
        this.update = update;
        this.delete = delete;
    }

    public static IReadOnlyDictionary<string, SampleDataManager> BuildTable(Type mapper)
    {
        // Find required methods on IMapper.
        MethodInfo? updateMethod = null, deleteMethod = null;

        foreach (var method in mapper.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (IsUpdateMethod(method))
            {
                updateMethod = method;
            }
            else if (IsDeleteMethod(method))
            {
                deleteMethod = method;
            }
        }

        if (updateMethod is null)
        {
            throw new ArgumentException("No method 'Task UpdateAsync<T>(T, CqlQueryOptions)'.", nameof(mapper));
        }
        else if (deleteMethod is null)
        {
            throw new ArgumentException("No method 'Task IMapper.DeleteAsync<T>(T, CqlQueryOptions)'.", nameof(mapper));
        }

        // Build table.
        var table = new Dictionary<string, SampleDataManager>();

        foreach (var type in typeof(ResumeSampleData<,>).Assembly.GetTypes())
        {
            // Check if type is sample data.
            Type? parent = null;

            if (type.IsAbstract)
            {
                continue;
            }

            for (var @base = type.BaseType; @base != null; @base = @base.BaseType)
            {
                if (!@base.IsGenericType)
                {
                    continue;
                }

                var definition = @base.GetGenericTypeDefinition();

                if (definition == typeof(ResumeSampleData<,>) || definition == typeof(MultiplicableSampleData<,>))
                {
                    parent = @base;
                    break;
                }
            }

            if (parent is null)
            {
                continue;
            }

            // Get a property that hold the data.
            var data = parent.GetProperty("Data");

            if (data is null)
            {
                throw new InvalidOperationException($"No property 'Data' has been defined on '{parent}'.");
            }

            // Build a manager.
            var newDto = () => (Models.IResumeSampleData)Activator.CreateInstance(type)!;
            var fetch = BuildFetch(parent);
            var update = BuildDtoAndCqlQueryOptionsDelegate(mapper, updateMethod, type);
            var delete = BuildDtoAndCqlQueryOptionsDelegate(mapper, deleteMethod, type);

            table.Add(DataObject.DomainTypes[data.PropertyType], new(newDto, fetch, update, delete));
        }

        return table;
    }

    public IResumeSampleData NewDto() => this.newDto.Invoke();

    public Task<IEnumerable<IResumeSampleData>> GetAsync(IMapper db, Guid userId, Guid? jobId, CultureInfo? culture, Action<CqlQueryOptions> options)
    {
        // Build query.
        Cql query;

        if (jobId is null && culture is null)
        {
            query = Cql.New("WHERE owner = ?", userId);
        }
        else if (jobId is null)
        {
            throw new ArgumentException($"The value cannot be null when '{nameof(culture)}' is not null.", nameof(jobId));
        }
        else if (culture is null)
        {
            query = Cql.New("WHERE owner = ? AND job = ?", userId, jobId.Value);
        }
        else
        {
            query = Cql.New("WHERE owner = ? AND job = ? AND culture = ?", userId, jobId.Value, culture.Name);
        }

        return this.fetch.Invoke(db, query.WithOptions(options));
    }

    public Task UpdateAsync(IMapper db, IResumeSampleData data, CqlQueryOptions options) => this.update.Invoke(db, data, options);

    public Task DeleteAsync(IMapper db, IResumeSampleData target, CqlQueryOptions options) => this.delete.Invoke(db, target, options);

    // Task IMapper.UpdateAsync<T>(T poco, CqlQueryOptions queryOptions = null)
    private static bool IsUpdateMethod(MethodInfo method) => method.Name == "UpdateAsync" && IsDtoAndCqlQueryOptionsMethod(method);

    // Task IMapper.DeleteAsync<T>(T poco, CqlQueryOptions queryOptions = null)
    private static bool IsDeleteMethod(MethodInfo method) => method.Name == "DeleteAsync" && IsDtoAndCqlQueryOptionsMethod(method);

    private static Func<IMapper, Cql, Task<IEnumerable<IResumeSampleData>>> BuildFetch(Type parent)
    {
        // Get fetcher method.
        var method = parent.GetMethod("FetchAsync", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        if (method is null)
        {
            throw new ArgumentException($"No method 'FetchAsync' has been defined on '{parent}'.", nameof(parent));
        }

        var @delegate = method.CreateDelegate<Func<IMapper, Cql, Task<IEnumerable<IResumeSampleData>>>>();

        // Wrap the delegate.
        var multiplicable = parent.GetGenericTypeDefinition() == typeof(MultiplicableSampleData<,>);

        return async (db, query) =>
        {
            var rows = await @delegate.Invoke(db, query);
            var groups = rows.GroupBy(r => (Job: r.TargetJob, Culture: CultureInfo.GetCultureInfo(r.Culture)));
            IEnumerable<IResumeSampleData> result;

            if (multiplicable)
            {
                result = groups.SelectMany(g => g.Cast<IMultiplicableSampleData>().OrderBy(r => r.Position));
            }
            else
            {
                result = groups.SelectMany(g => g);
            }

            return result.ToArray();
        };
    }

    private static Func<IMapper, IResumeSampleData, CqlQueryOptions, Task> BuildDtoAndCqlQueryOptionsDelegate(Type mapper, MethodInfo generic, Type dto)
    {
        // Create the following lambda: (IMapper db, IResumeSampleData row, CqlQueryOptions options) => ((mapper)(db)).method((dto)row, options)
        var method = generic.MakeGenericMethod(dto);
        var db = Expression.Parameter(typeof(IMapper), "db");
        var row = Expression.Parameter(typeof(IResumeSampleData), "row");
        var options = Expression.Parameter(typeof(CqlQueryOptions), "options");
        var lambda = Expression.Lambda<Func<IMapper, IResumeSampleData, CqlQueryOptions, Task>>(
            Expression.Call(Expression.Convert(db, mapper), method, Expression.Convert(row, dto), options),
            db,
            row,
            options);

        return lambda.Compile();
    }

    private static bool IsDtoAndCqlQueryOptionsMethod(MethodInfo method)
    {
        // Check if method is a generic method, have exactly 1 type parameter and have Task as a return.
        if (!method.IsGenericMethodDefinition || method.GetGenericArguments().Length != 1 || method.ReturnType != typeof(Task))
        {
            return false;
        }

        // Check if method have exactly 2 parameters.
        var parameters = method.GetParameters();

        if (parameters.Length != 2)
        {
            return false;
        }

        // Check if the first parameter is a generic parameter.
        var first = parameters[0];

        if (!first.ParameterType.IsGenericParameter || first.ParameterType.GenericParameterPosition != 0)
        {
            return false;
        }

        // Check if the second parameter is CqlQueryOptions.
        var second = parameters[1];

        if (second.ParameterType != typeof(CqlQueryOptions))
        {
            return false;
        }

        return true;
    }
}
