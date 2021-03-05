using EtherMon.Services;
using Xamarin.Forms;

namespace EtherMon
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<AddressesDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
