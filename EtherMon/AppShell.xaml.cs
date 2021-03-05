using System;
using EtherMon.Views;
using Xamarin.Forms;

namespace EtherMon
{
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(FrontPage), typeof(FrontPage));
        }

        private async void OnSettingsItemClicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = false;
            await Current.GoToAsync($"/{nameof(SettingsPage)}");
        }
    }
}
