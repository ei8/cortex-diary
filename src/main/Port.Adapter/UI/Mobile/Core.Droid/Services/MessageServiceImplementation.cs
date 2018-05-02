using Android.Widget;
using works.ei8.Cortex.Diary.Application.Message;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(MessageServiceImplementation))]
namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Droid.Services
{    
    public class MessageServiceImplementation : IMessageServiceImplementation
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}