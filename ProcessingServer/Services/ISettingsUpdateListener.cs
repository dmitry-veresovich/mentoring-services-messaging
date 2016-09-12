namespace ProcessingServer.Services
{
    internal interface ISettingsUpdateListener
    {
        void Listen();
        void Stop();
    }
}
