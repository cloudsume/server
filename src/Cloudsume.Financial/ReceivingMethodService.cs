namespace Cloudsume.Financial
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Ultima.Extensions.Collections;

    internal sealed class ReceivingMethodService : IReceivingMethodService
    {
        private readonly SortedDictionary<Type, IReceivingMethodServiceAdaptor> receivingIndex;
        private readonly SortedDictionary<Type, IReceivingMethodServiceAdaptor> paymentIndex;

        public ReceivingMethodService(IEnumerable<IReceivingMethodServiceAdaptor> adaptors)
        {
            this.receivingIndex = new(TypeComparer.Derived);
            this.paymentIndex = new(TypeComparer.Derived);

            foreach (var adaptor in adaptors)
            {
                this.receivingIndex.Add(adaptor.ReceivingType, adaptor);
                this.paymentIndex.Add(adaptor.PaymentType, adaptor);
            }
        }

        public Task CancelPaymentAsync(PaymentMethod method, PaymentCancelReason reason, CancellationToken cancellationToken = default)
        {
            return this.paymentIndex[method.GetType()].CancelPaymentAsync(method, reason, cancellationToken);
        }

        public Task<PaymentMethod?> CreatePaymentMethodAsync(
            ReceivingMethod method,
            PaymentInfo info,
            decimal fee,
            CancellationToken cancellationToken = default)
        {
            if (fee < 0m || fee > info.Amount)
            {
                throw new ArgumentOutOfRangeException(nameof(fee));
            }
            else if (method.UserId == Guid.Empty && fee != 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(fee));
            }

            return this.receivingIndex[method.GetType()].CreatePaymentMethodAsync(method, info, fee, cancellationToken);
        }

        public Task<Uri?> GetSetupUriAsync(ReceivingMethod method, Uri returnUri, CancellationToken cancellationToken = default)
        {
            return this.receivingIndex[method.GetType()].GetSetupUriAsync(method, returnUri, cancellationToken);
        }

        public Task<PaymentMethodStatus> GetStatusAsync(PaymentMethod method, CancellationToken cancellationToken = default)
        {
            return this.paymentIndex[method.GetType()].GetStatusAsync(method, cancellationToken);
        }

        public Task<ReceivingMethodStatus> GetStatusAsync(ReceivingMethod method, CancellationToken cancellationToken = default)
        {
            return this.receivingIndex[method.GetType()].GetStatusAsync(method, cancellationToken);
        }
    }
}
