namespace NetCaptcha;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ultima.Extensions.Collections;

internal sealed class CaptchaService : ICaptchaService
{
    private readonly SortedDictionary<Type, ICaptchaValidator> validators;

    public CaptchaService(IEnumerable<ICaptchaValidator> validators)
    {
        if (!validators.Any())
        {
            throw new ArgumentException("No any available CAPTCHA validators. Perhaps you forgot to add CAPTCHA provider?", nameof(validators));
        }

        this.validators = new(TypeComparer.Derived);

        foreach (var validator in validators)
        {
            this.validators.Add(validator.ParamsType, validator);
        }
    }

    public ValueTask<bool> ValidateAsync(ValidationParams @params, CancellationToken cancellationToken = default)
    {
        return this.validators[@params.GetType()].ExecuteAsync(@params, cancellationToken);
    }
}
