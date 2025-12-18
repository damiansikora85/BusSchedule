#if ANDROID
//using Firebase.Crashlytics;
//using Firebase.Analytics;
#endif
using System.Collections.Generic;

namespace BusSchedule.Core.Services;

public class FirebaseAnalyticsService : IAnalyticsService
{
    public void LogEvent(string eventName, IDictionary<string, string> parameters = null)
    {
#if ANDROID
        //if (parameters == null)
        //{
        //    FirebaseAnalytics.GetInstance(Platform.CurrentActivity).LogEvent(eventName, null);
        //}
        //else
        //{
        //    var bundle = new Android.OS.Bundle();
        //    foreach (var param in parameters)
        //    {
        //        bundle.PutString(param.Key, param.Value);
        //    }
        //    FirebaseAnalytics.GetInstance(Platform.CurrentActivity).LogEvent(eventName, bundle);
        //}
#endif
    }

    public void LogException(Exception exception)
    {
#if ANDROID
        //FirebaseCrashlytics.Instance.RecordException(Java.Lang.Throwable.FromException(exception));
#endif
    }

    public void LogException(Exception exception, IDictionary<string, string> parameters)
    {
//#if ANDROID
//        var throwable = Java.Lang.Throwable.FromException(exception);
//        var keysAndValues = new Dictionary<string, string>();
//        foreach (var param in parameters)
//        {
//            keysAndValues[param.Key] = param.Value;
//        }
//        FirebaseCrashlytics.Instance.RecordException(throwable, keysAndValues);
//#endif
    }

    public void SetUserId(string userId)
    {
#if ANDROID
        //FirebaseAnalytics.GetInstance(Platform.CurrentActivity).SetUserId(userId);
#endif
    }

    public void SetUserProperty(string name, string value)
    {
#if ANDROID
        //FirebaseAnalytics.GetInstance(Platform.CurrentActivity).SetUserProperty(name, value);
#endif
    }
}
