namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cloudsume.Server.Cassandra;
using global::Cassandra;
using global::Cassandra.Mapping;

public sealed class ResumeDataManager
{
    private readonly Func<IResumeData> newDto;
    private readonly Func<IMapper, IResumeData, CqlQueryOptions, Task> update;
    private readonly Func<IMapper, Guid, Guid, ConsistencyLevel, Task<object?>> get;
    private readonly Func<IMapper, Cql, Task<IEnumerable<IResumeData>>> fetch;
    private readonly Func<IMapper, IResumeData, CqlQueryOptions, Task> delete;
    private readonly Func<IMapper, Guid, Guid, Task> clear;

    private ResumeDataManager(
        Func<IResumeData> newDto,
        Func<IMapper, IResumeData, CqlQueryOptions, Task> update,
        Func<IMapper, Guid, Guid, ConsistencyLevel, Task<object?>> get,
        Func<IMapper, Cql, Task<IEnumerable<IResumeData>>> fetch,
        Func<IMapper, IResumeData, CqlQueryOptions, Task> delete,
        Func<IMapper, Guid, Guid, Task> clear)
    {
        this.newDto = newDto;
        this.update = update;
        this.get = get;
        this.fetch = fetch;
        this.delete = delete;
        this.clear = clear;
    }

    public static IReadOnlyDictionary<string, ResumeDataManager> BuildTable(Type mapper)
    {
        // Find required methods on IMapper.
        MethodInfo? updateMethod = null, deleteMethod = null, clearMethod = null;

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
            else if (IsClearMethod(method))
            {
                clearMethod = method;
            }
        }

        if (updateMethod == null)
        {
            throw new ArgumentException("No method 'Task UpdateAsync<T>(T, CqlQueryOptions)'.", nameof(mapper));
        }
        else if (deleteMethod == null)
        {
            throw new ArgumentException("No method 'Task IMapper.DeleteAsync<T>(T, CqlQueryOptions)'.", nameof(mapper));
        }
        else if (clearMethod == null)
        {
            throw new ArgumentException("No method 'Task DeleteAsync<T>(string, params object[])'.", nameof(mapper));
        }

        // Build table.
        var table = new Dictionary<string, ResumeDataManager>();

        foreach (var type in typeof(Models.ResumeData<,>).Assembly.GetTypes())
        {
            // Check if type is resume data.
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

                if (definition == typeof(Models.MultiplicableResumeData<,>) || definition == typeof(Models.ResumeData<,>))
                {
                    parent = @base;
                    break;
                }
            }

            if (parent == null)
            {
                continue;
            }

            // Get a property that hold the data.
            var data = parent.GetProperty("Data");

            if (data == null)
            {
                throw new InvalidOperationException($"No property 'Data' has been defined on '{parent}'.");
            }

            // Build a manager.
            var newDto = () => (IResumeData)Activator.CreateInstance(type)!;
            var update = BuildDtoAndCqlQueryOptionsDelegate(mapper, updateMethod, type);
            var get = BuildGet(parent);
            var fetch = BuildFetch(parent);
            var delete = BuildDtoAndCqlQueryOptionsDelegate(mapper, deleteMethod, type);
            var clear = BuildClear(mapper, clearMethod, type);

            table.Add(Models.DataObject.DomainTypes[data.PropertyType], new(newDto, update, get, fetch, delete, clear));
        }

        return table;
    }

    public IResumeData NewDto() => this.newDto.Invoke();

    public Task UpdateAsync(IMapper db, IResumeData row, CqlQueryOptions options) => this.update.Invoke(db, row, options);

    public Task<object?> GetAsync(IMapper db, Guid userId, Guid resumeId, ConsistencyLevel consistency) => this.get.Invoke(db, userId, resumeId, consistency);

    public Task<IEnumerable<IResumeData>> GetAsync(IMapper db, Cql cql) => this.fetch.Invoke(db, cql);

    public Task DeleteAsync(IMapper db, IResumeData row, CqlQueryOptions options) => this.delete.Invoke(db, row, options);

    public Task ClearAsync(IMapper db, Guid userId, Guid resumeId) => this.clear.Invoke(db, userId, resumeId);

    // Task IMapper.UpdateAsync<T>(T poco, CqlQueryOptions queryOptions = null)
    private static bool IsUpdateMethod(MethodInfo method) => method.Name == "UpdateAsync" && IsDtoAndCqlQueryOptionsMethod(method);

    // Task IMapper.DeleteAsync<T>(T poco, CqlQueryOptions queryOptions = null)
    private static bool IsDeleteMethod(MethodInfo method) => method.Name == "DeleteAsync" && IsDtoAndCqlQueryOptionsMethod(method);

    // Task IMapper.DeleteAsync<T>(string cql, params object[] args)
    private static bool IsClearMethod(MethodInfo method)
    {
        if (method.Name != "DeleteAsync")
        {
            return false;
        }

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

        // Check if the first parameter is a string.
        var first = parameters[0];

        if (first.ParameterType != typeof(string))
        {
            return false;
        }

        // Check if the second parameter is 'params object[]'.
        var second = parameters[1];

        if (!second.IsDefined(typeof(ParamArrayAttribute)) || !second.ParameterType.IsArray || second.ParameterType.GetElementType() != typeof(object))
        {
            return false;
        }

        return true;
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

    private static Func<IMapper, IResumeData, CqlQueryOptions, Task> BuildDtoAndCqlQueryOptionsDelegate(Type mapper, MethodInfo generic, Type dto)
    {
        // Create the following lambda: (IMapper db, IResumeData row, CqlQueryOptions options) => ((mapper)(db)).method((dto)row, options)
        var method = generic.MakeGenericMethod(dto);
        var db = Expression.Parameter(typeof(IMapper), "db");
        var row = Expression.Parameter(typeof(IResumeData), "row");
        var options = Expression.Parameter(typeof(CqlQueryOptions), "options");
        var @delegate = Expression.Lambda<Func<IMapper, IResumeData, CqlQueryOptions, Task>>(
            Expression.Call(Expression.Convert(db, mapper), method, Expression.Convert(row, dto), options),
            db,
            row,
            options).Compile();

        return (db, row, options) => @delegate.Invoke(db, row, options);
    }

    private static Func<IMapper, Guid, Guid, ConsistencyLevel, Task<object?>> BuildGet(Type parent)
    {
        // Get the method.
        var @params = new[] { typeof(IMapper), typeof(Guid), typeof(Guid), typeof(ConsistencyLevel) };
        var method = parent.GetMethod("FetchAsync", BindingFlags.Public | BindingFlags.Static, @params);

        if (method == null)
        {
            throw new ArgumentException($"No method 'FetchAsync({string.Join<Type>(", ", @params)})'.", nameof(parent));
        }

        // Create a delegate.
        if (parent.GetGenericTypeDefinition() == typeof(Models.MultiplicableResumeData<,>))
        {
            var @delegate = method.CreateDelegate<Func<IMapper, Guid, Guid, ConsistencyLevel, Task<IEnumerable<IMultiplicativeResumeData>>>>();

            return async (db, userId, resumeId, consistency) => await @delegate.Invoke(db, userId, resumeId, consistency);
        }
        else
        {
            var @delegate = method.CreateDelegate<Func<IMapper, Guid, Guid, ConsistencyLevel, Task<IResumeData?>>>();

            return async (db, userId, resumeId, consistency) => await @delegate.Invoke(db, userId, resumeId, consistency);
        }
    }

    private static Func<IMapper, Cql, Task<IEnumerable<IResumeData>>> BuildFetch(Type parent)
    {
        // Get the method.
        var @params = new[] { typeof(IMapper), typeof(Cql) };
        var method = parent.GetMethod("FetchAsync", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, @params);

        if (method == null)
        {
            throw new ArgumentException($"No method 'FetchAsync({string.Join<Type>(", ", @params)})'.", nameof(parent));
        }

        // Create a delegate.
        var @delegate = method.CreateDelegate<Func<IMapper, Cql, Task<IEnumerable<IResumeData>>>>();

        return (db, cql) => @delegate.Invoke(db, cql);
    }

    private static Func<IMapper, Guid, Guid, Task> BuildClear(Type mapper, MethodInfo generic, Type dto)
    {
        // (IMapper db, string cql, object[] args) => db.method(cql, args)
        var method = generic.MakeGenericMethod(dto);
        var db = Expression.Parameter(typeof(IMapper), "db");
        var cql = Expression.Parameter(typeof(string), "cql");
        var args = Expression.Parameter(typeof(object[]), "args");
        var @delegate = Expression.Lambda<Func<IMapper, string, object[], Task>>(
            Expression.Call(Expression.Convert(db, mapper), method, cql, args),
            db,
            cql,
            args).Compile();

        return (db, userId, resumeId) => @delegate.Invoke(db, "WHERE user_id = ? AND resume_id = ?", new object[] { userId, resumeId });
    }
}
