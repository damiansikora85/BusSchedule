namespace BusSchedule.Core.Services
{
    public interface IAnalyticsService
    {
        void LogEvent(string eventName, IDictionary<string, string> parameters = null);
        void LogException(Exception exception);
        void SetUserId(string userId);
        void SetUserProperty(string name, string value);
    }
}
