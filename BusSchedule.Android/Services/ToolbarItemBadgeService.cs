﻿//using BusSchedule.Droid.Services;
//using BusSchedule.Droid.Utils;
//using BusSchedule.Interfaces;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;

//[assembly: Dependency(typeof(ToolbarItemBadgeService))]
//namespace BusSchedule.Droid.Services
//{
//    public class ToolbarItemBadgeService : IToolbarItemBadgeService
//    {
//        public void SetBadge(Page page, ToolbarItem item, string value, Color backgroundColor, Color textColor)
//        {
//            Device.BeginInvokeOnMainThread(() =>
//            {
//                var toolbar = Xamarin.Essentials.Platform.CurrentActivity.FindViewById(Resource.Id.toolbar) as AndroidX.AppCompat.Widget.Toolbar;
//                if (toolbar != null)
//                {
//                    if (!string.IsNullOrEmpty(value))
//                    {
//                        var idx = page.ToolbarItems.IndexOf(item);
//                        if (toolbar.Menu.Size() > idx)
//                        {
//                            var menuItem = toolbar.Menu.GetItem(idx);
//                            BadgeDrawable.SetBadgeText(Xamarin.Essentials.Platform.CurrentActivity, menuItem, value, backgroundColor.ToAndroid(), textColor.ToAndroid());
//                        }
//                    }
//                }
//            });
//        }
//    }
//}