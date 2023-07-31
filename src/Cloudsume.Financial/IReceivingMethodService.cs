namespace Cloudsume.Financial
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IReceivingMethodService
    {
        /// <summary>
        /// Create a payment method to receive a payment for a specified receiving method.
        /// </summary>
        /// <param name="method">
        /// A receiving method to create a payment method.
        /// </param>
        /// <param name="info">
        /// Payment information to create a payment method.
        /// </param>
        /// <param name="fee">
        /// Commision fee to take.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A non-referenced payment method; that is, <see cref="PaymentMethod.IsReference"/> is <c>false</c> or <c>null</c> if <paramref name="method"/> cannot
        /// accept a payment in the specified currency or amount.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="fee"/> is genative, higher than <see cref="PaymentInfo.Amount"/> or not zero while <paramref name="method"/> is owned by a system.
        /// </exception>
        Task<PaymentMethod?> CreatePaymentMethodAsync(ReceivingMethod method, PaymentInfo info, decimal fee, CancellationToken cancellationToken = default);

        Task CancelPaymentAsync(PaymentMethod method, PaymentCancelReason reason, CancellationToken cancellationToken = default);

        Task<ReceivingMethodStatus> GetStatusAsync(ReceivingMethod method, CancellationToken cancellationToken = default);

        Task<PaymentMethodStatus> GetStatusAsync(PaymentMethod method, CancellationToken cancellationToken = default);

        Task<Uri?> GetSetupUriAsync(ReceivingMethod method, Uri returnUri, CancellationToken cancellationToken = default);
    }
}
