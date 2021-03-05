using System;
using EtherMon.Views;
using Xamarin.Forms;

namespace EtherMon
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(FrontPage), typeof(FrontPage));
        }

        private async void OnSettingsItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync($"/{nameof(SettingsPage)}");
        }
    }
}
