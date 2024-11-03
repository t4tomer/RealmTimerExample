using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using RealmTodo.Views;

using Realms;
using System.Windows.Input;
//using AddressBookUI;

namespace RealmTodo.ViewModels
{
    public partial class UserRecordViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllUserRecords;

        [ObservableProperty]
        private IQueryable<UserRecord> user_records;

        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;
        public ICommand NavigateCommand { get; private set; }


        [RelayCommand]
        public void OnAppearing()
        {
            Item newItem = new Item();
            realm = RealmService.GetMainThreadRealm(newItem);
            currentUserId = RealmService.CurrentUser.Id;
            User_records = realm.All<UserRecord>().OrderBy(i => i.Id);

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);
            IsShowAllUserRecords = currentSubscriptionType == SubscriptionType.All;
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
        public async Task AddUserRecord()
        {
            await Shell.Current.GoToAsync($"userRecordEdit");
        }

        [RelayCommand]
        public async Task ToTimerPage()
        {
            // Navigate to the singleton instance of MapPage
            var timerPage = TimerPage.Instance;
            await Shell.Current.Navigation.PushAsync(timerPage);            
        }





        [RelayCommand]
        public async Task EditUserRecord(UserRecord userRecord)
        {
            if (!await CheckUserRecordOwnership(userRecord))
            {
                return;
            }
            var userRecordParameter = new Dictionary<string, object>() { { "userRecord", userRecord } };
            await Shell.Current.GoToAsync($"userRecordEdit", userRecordParameter);
        }

        [RelayCommand]
        public async Task DeleteUserRecord(UserRecord userRecord)
        {
            if (!await CheckUserRecordOwnership(userRecord))
            {
                return;
            }

            await realm.WriteAsync(() =>
            {
                realm.Remove(userRecord);
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

        private async Task<bool> CheckUserRecordOwnership(UserRecord UserRecord)
        {
            if (!UserRecord.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot modify UserRecords not belonging to you", "OK");
                return false;
            }

            return true;
        }


    }
}

