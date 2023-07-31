namespace NetCaptcha;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides base implementation of <see cref="ICaptchaValidator"/>.
/// </summary>
/// <typeparam name="TParams">
/// An implementation of <see cref="ValidationParams"/> the derived class can handled.
/// </typeparam>
public abstract class CaptchaValidator<TParams> : ICaptchaValidator
    where TParams : ValidationParams
{
    public Type ParamsType => typeof(TParams);

    public abstract ValueTask<bool> ExecuteAsync(TParams @params, CancellationToken cancellationToken = default);

    ValueTask<bool> ICaptchaValidator.ExecuteAsync(ValidationParams @params, CancellationToken cancellationToken)
    {
        return this.ExecuteAsync((TParams)@params, cancellationToken);
    }
}
