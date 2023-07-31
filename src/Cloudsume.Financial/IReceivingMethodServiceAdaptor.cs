namespace Cloudsume.Financial
{
    using System;

    public interface IReceivingMethodServiceAdaptor : IReceivingMethodService
    {
        Type ReceivingType { get; }

        Type PaymentType { get; }
    }
}
