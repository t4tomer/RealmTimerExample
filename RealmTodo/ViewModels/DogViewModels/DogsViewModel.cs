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
    public partial class DogsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<Item> items;


        [ObservableProperty]
        private IQueryable<Dog> dogs;



        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;
        public ICommand NavigateCommand { get; private set; }


        [RelayCommand]
        public void OnAppearing()
        {
            Console.WriteLine("----> (OnAppearing-DogsViewModel 1)");


            //set the singlton object to dog type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetDogType();


            realm = RealmService.GetMainThreadRealm();

            // Check if the subscription for Dog type exists
            var dogSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

            if (!dogSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var dogQuery = realm.All<Dog>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(dogQuery, new SubscriptionOptions { Name = "DogSubscription" });
                });

                Console.WriteLine("Dog subscription added. Waiting for synchronization...");

                // Wait for synchronization
                realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Dog subscription already exists.");
            }




            currentUserId = RealmService.CurrentUser.Id;
            Dogs = realm.All<Dog>().OrderBy(i => i.Id);
            
            Console.WriteLine("----> (OnAppearing-DogsViewModel 2)");


            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);

            // Print the Summary of each Dog
            Console.WriteLine("----> Printing Names of Dogs:");
            foreach (var dog in Dogs)
            {
                Console.WriteLine($"Dog Name: {dog.Name}");
            }





            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;
        }

        [RelayCommand]
        public void Refresh()
        {
            Console.WriteLine($"---> refreshed page (DogsList) ");
            OnAppearing();

        }


        [RelayCommand]
        public async Task AddItem()
        {
            Console.WriteLine($"-->AddItem (DogsViewModel)");

            await Shell.Current.GoToAsync($"itemEdit");
        }


        [RelayCommand]
        public async Task AddMapPin()
        {
            //set the singlton object to mapin type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetMapPinType();

            var realm = RealmService.GetMainThreadRealm();

            Console.WriteLine($"-->AddMapPin (DogsViewModel)");

            await Shell.Current.GoToAsync($"mapPinEdit");
            //var editDogPage = new EditDogPage();
            //await Shell.Current.Navigation.PushAsync(editDogPage);
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
        public async Task AddDog()
        {

            var realm = RealmService.GetMainThreadRealm();

            Console.WriteLine($"-->AddDog (DogsViewModel)");

            await Shell.Current.GoToAsync($"dogEdit");

        }

        [RelayCommand]
        public async Task DeleteDog(Dog dog)
        {
            //if (!await CheckDogOwnership(dog))
            //{
            //    return;
            //}

            await realm.WriteAsync(() =>
            {
                realm.Remove(dog);
            });
        }


        [RelayCommand]
        public async Task EditDog(Dog dog)
        {
            // Implement navigation or logic for editing a dog
            if (dog == null)
            {
                Console.WriteLine("Dog parameter is null.");
                return;
            }

            Console.WriteLine($"Editing dog: {dog.Name}");
            var dogParameter = new Dictionary<string, object> { { "dog", dog } };
            await Shell.Current.GoToAsync("dogEdit", dogParameter);
        }







        [RelayCommand]
        public async Task ToTimerPage()//transfer to timer page
        {
            // Navigate to the singleton instance of MapPage
            var timerPage = TimerPage.Instance;
            await Shell.Current.Navigation.PushAsync(timerPage);            
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

