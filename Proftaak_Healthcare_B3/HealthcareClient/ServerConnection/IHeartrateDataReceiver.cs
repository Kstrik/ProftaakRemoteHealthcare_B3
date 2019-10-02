namespace HealthcareClient.ServerConnection
{
    public interface IHeartrateDataReceiver
    {
        void ReceiveHeartrateData(int heartrate, HeartrateMonitor heartrateMonitor);
    }
}