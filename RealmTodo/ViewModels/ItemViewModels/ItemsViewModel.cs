using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using RealmTodo.Views;

using Realms;
using System.Windows.Input;
using Realms.Sync;

namespace RealmTodo.ViewModels
{
    public partial class ItemsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<Item> items;



        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;

        [RelayCommand]
        public void OnAppearing()
        {
            //set the singlton object to item type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetItemType();


            realm = RealmService.GetMainThreadRealm();


            // Check if the subscription for Item type exists
            var itemSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "ItemSubscription");

            if (!itemSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Item. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var itemQuery = realm.All<Item>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(itemQuery, new SubscriptionOptions { Name = "ItemSubscription" });
                });

                Console.WriteLine("Item subscription added. Waiting for synchronization...");

                // Wait for synchronization
                realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Item subscription already exists.");
            }




            currentUserId = RealmService.CurrentUser.Id;
            Items = realm.All<Item>().OrderBy(i => i.Id);

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);


            // Print the Summary of each Item
            Console.WriteLine("----> Printing Summaries of Items:");
            foreach (var item in Items)
            {
                Console.WriteLine($"Item Summary: {item.Summary}");
            }




            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;




        }


        [RelayCommand]
        public void Refresh()
        {
            Console.WriteLine($"---> refreshed page (ItemsList) ");
            OnAppearing();

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
            Console.WriteLine($"-->AddItem (ItemsViewModel)");

            await Shell.Current.GoToAsync($"itemEdit");
        }

        [RelayCommand]
        public async Task AddDog()
        {
            Console.WriteLine($"-->AddDog (ItemsViewModel)");

            await Shell.Current.GoToAsync($"dogEdit");
        }



        [RelayCommand]
        public async Task AddMapPin()
        {
            //set the singlton object to mapin type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetMapPinType();

            var realm = RealmService.GetMainThreadRealm();

            Console.WriteLine($"-->AddMapPin (ItemsViewModel)");

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
        public async Task EditItem(Item item)
        {
            if (!await CheckItemOwnership(item))
            {
                return;
            }
            var itemParameter = new Dictionary<string, object>() { { "item", item } };
            await Shell.Current.GoToAsync($"itemEdit", itemParameter);
        }

        [RelayCommand]
        public async Task DeleteItem(Item item)
        {
            if (!await CheckItemOwnership(item))
            {
                return;
            }

            await realm.WriteAsync(() =>
            {
                realm.Remove(item);
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

        private async Task<bool> CheckItemOwnership(Item item)
        {
            if (!item.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot modify items not belonging to you", "OK");
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

