namespace Cloudsume.Financial
{
    public enum ReceivingMethodStatus
    {
        SetupRequired = 0x00000000,
        Processing = 0x00000100,
        ActionRequired = 0x00010000,
        Ready = 0x01000000,
    }
}
