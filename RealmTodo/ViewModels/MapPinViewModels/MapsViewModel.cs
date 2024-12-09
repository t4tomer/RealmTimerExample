using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using RealmTodo.Views;

using Realms;
using System.Windows.Input;

namespace RealmTodo.ViewModels
{
    public partial class MapsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;



        [ObservableProperty]
        private IQueryable<MapPin> maps;




        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;

        [RelayCommand]
        public void OnAppearing()
        {
            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
            Maps = realm.All<MapPin>().OrderBy(i => i.Id);

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);
            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;
        }

        [RelayCommand]
        public async Task Logout()
        {
            IsBusy = true;
            await RealmService.LogoutAsync();
            IsBusy = false;

            await Shell.Current.GoToAsync($"//login");
        }

        [RelayCommand]
        public async Task AddItem()
        {
            Console.WriteLine($"-->AddItem (MapsViewModel)");

            await Shell.Current.GoToAsync($"itemEdit");
        }

        [RelayCommand]
        public async Task AddDog()
        {
            Console.WriteLine($"-->AddDog (MapsViewModel)");

            await Shell.Current.GoToAsync($"dogEdit");
        }



        [RelayCommand]
        public async Task AddMapPin()
        {

            var realm = RealmService.GetMainThreadRealm();

            Console.WriteLine($"-->AddMapPin (MapsViewModel)");

            await Shell.Current.GoToAsync($"mapPinEdit");
            //var editDogPage = new EditDogPage();
            //await Shell.Current.Navigation.PushAsync(editDogPage);
        }





        [RelayCommand]
        public async Task ToTimerPage()//transfer to timer page
        {
            // Navigate to the singleton instance of TimerPAge
            var timerPage = TimerPage.Instance;
            await Shell.Current.Navigation.PushAsync(timerPage);
        }





        [RelayCommand]
        public async Task EditMapPin(MapPin pin)
        {
            if (!await CheckItemOwnershipMapPin(pin))
            {
                return;
            }
            var mapPinParameter = new Dictionary<string, object>() { { "mappin",pin } };
            await Shell.Current.GoToAsync($"mapPinEdit", mapPinParameter);
        }

        [RelayCommand]
        public async Task DeleteMapPin(MapPin pin)
        {
            //if (!await CheckItemOwnershipMapPin(pin))
            //{
            //    return;
            //}
            Console.WriteLine($"-->deleting map pin (MapsViewModel)");

            await realm.WriteAsync(() =>
            {
                realm.Remove(pin);
            });
        }




        [RelayCommand]
        public void ChangeConnectionStatus()
        {
            isOnline = !isOnline;

            if (isOnline)
            {
                realm.SyncSession.Start();
            }
            else
            {
                realm.SyncSession.Stop();
            }

            ConnectionStatusIcon = isOnline ? "wifi_on.png" : "wifi_off.png";
        }

        [RelayCommand]
        public async Task UrlTap(string url)
        {
            await Launcher.OpenAsync(DataExplorerLink);
        }

        private async Task<bool> CheckItemOwnershipMapPin(MapPin pin)
        {
            if (!pin.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot modify pins not belonging to you", "OK");
                return false;
            }

            return true;
        }

        private async Task<bool> CheckDogOwnership(Dog dog)
        {
            if (!dog.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot modify dog not belonging to you", "OK");
                return false;
            }

            return true;
        }






        async partial void OnIsShowAllTasksChanged(bool value)
        {
            await RealmService.SetSubscription(realm, value ? SubscriptionType.All : SubscriptionType.Mine);

            if (!isOnline)
            {
                await DialogService.ShowToast("Switching subscriptions does not affect Realm data when the sync is offline.");
            }
        }
    }
}

