namespace Cloudsume.Financial
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ReceivingMethodServiceAdaptor<TReceiving, TPayment> : IReceivingMethodServiceAdaptor
        where TReceiving : ReceivingMethod
        where TPayment : PaymentMethod
    {
        public Type PaymentType => typeof(TPayment);

        public Type ReceivingType => typeof(TReceiving);

        public abstract Task CancelPaymentAsync(TPayment method, PaymentCancelReason reason, CancellationToken cancellationToken);

        public abstract Task<PaymentMethod?> CreatePaymentMethodAsync(
            TReceiving method,
            PaymentInfo info,
            decimal fee,
            CancellationToken cancellationToken = default);

        public abstract Task<Uri?> GetSetupUriAsync(TReceiving method, Uri returnUri, CancellationToken cancellationToken = default);

        public abstract Task<PaymentMethodStatus> GetStatusAsync(TPayment method, CancellationToken cancellationToken = default);

        public abstract Task<ReceivingMethodStatus> GetStatusAsync(TReceiving method, CancellationToken cancellationToken = default);

        Task IReceivingMethodService.CancelPaymentAsync(PaymentMethod method, PaymentCancelReason reason, CancellationToken cancellationToken)
        {
            return this.CancelPaymentAsync((TPayment)method, reason, cancellationToken);
        }

        Task<PaymentMethod?> IReceivingMethodService.CreatePaymentMethodAsync(
            ReceivingMethod method,
            PaymentInfo info,
            decimal fee,
            CancellationToken cancellationToken)
        {
            return this.CreatePaymentMethodAsync((TReceiving)method, info, fee, cancellationToken);
        }

        Task<Uri?> IReceivingMethodService.GetSetupUriAsync(ReceivingMethod method, Uri returnUri, CancellationToken cancellationToken)
        {
            return this.GetSetupUriAsync((TReceiving)method, returnUri, cancellationToken);
        }

        Task<PaymentMethodStatus> IReceivingMethodService.GetStatusAsync(PaymentMethod method, CancellationToken cancellationToken)
        {
            return this.GetStatusAsync((TPayment)method, cancellationToken);
        }

        Task<ReceivingMethodStatus> IReceivingMethodService.GetStatusAsync(ReceivingMethod method, CancellationToken cancellationToken)
        {
            return this.GetStatusAsync((TReceiving)method, cancellationToken);
        }
    }
}
