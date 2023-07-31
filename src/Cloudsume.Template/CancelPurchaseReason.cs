namespace Cloudsume.Template;

public enum CancelPurchaseReason : sbyte
{
    TooExpensive = 0,
    NoCard = 1,
    Untrusted = 2,
    MistakenFree = 3,
    PaymentNotWorking = 4,
}
