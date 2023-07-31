namespace NetCaptcha;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface ICaptchaValidator
{
    /// <summary>
    /// Gets a type of <see cref="ValidationParams"/> this implementation can handled.
    /// </summary>
    /// <remarks>
    /// Derived type also handled.
    /// </remarks>
    Type ParamsType { get; }

    ValueTask<bool> ExecuteAsync(ValidationParams @params, CancellationToken cancellationToken = default);
}
