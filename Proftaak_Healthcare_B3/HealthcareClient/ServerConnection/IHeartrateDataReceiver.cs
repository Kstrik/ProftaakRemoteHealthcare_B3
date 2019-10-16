namespace HealthcareClient.ServerConnection
{
    public interface IHeartrateDataReceiver
    {
        void ReceiveHeartrateData(byte heartrate);
    }
}