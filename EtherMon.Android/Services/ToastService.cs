using Android.App;
using Android.Widget;
using EtherMon.Droid.Services;
using EtherMon.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ToastService))]
namespace EtherMon.Droid.Services
{
    public class ToastService: IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long)?.Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
        }
    }
}