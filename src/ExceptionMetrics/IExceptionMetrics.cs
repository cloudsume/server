namespace ExceptionMetrics;

using System;
using System.Threading.Tasks;

public interface IExceptionMetrics
{
    Task<object?> WriteAsync(Exception exception);
}
