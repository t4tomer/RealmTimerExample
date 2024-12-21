using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using System.Text.Json;
using Realms;
using Realms.Sync;
using RealmTodo.Models;
using RealmTodo.Views;

namespace RealmTodo.ViewModels
{
    public partial class EditDogViewModel : BaseViewModel, IQueryAttributable
    {



        [ObservableProperty]
        private string summary;

        [ObservableProperty]
        private string pageHeader;

        private static Realms.Sync.App app;

        private string currentUserId = RealmService.CurrentUser.Id;

        // dog class 
        [ObservableProperty]
        private Dog initialDog;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int age;

        // dog class 






        //// ApplyQueryAttributes-dog class 
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Console.WriteLine($"-->ApplyQueryAttributes method (EditDogViewModel)");

            if (query.Count > 0 && query["dog"] != null) // we're editing an Item
            {
                InitialDog = query["dog"] as Dog;
                Name = InitialDog.Name;
                Age = InitialDog.Age;
                PageHeader = $"Modify Dog {InitialDog.Id}";
            }
            else // we're creating a new item
            {
                Name = "nameTest";
                Age = 0;
            }
        }





        [RelayCommand]
        public async Task SaveDog()
        {
            Console.WriteLine($"--> SaveDog method (EditDogViewModel)");
            //set the singlton object to mapin type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetDogType();

            // Get the Realm instance
            var realm = RealmService.GetMainThreadRealm();

            //this check fixed the problem of no flexibale subscrption !!!!

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
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Dog subscription already exists.");
            }

            // Proceed with adding the Dog object
            await realm.WriteAsync(() =>
            {
                realm.Add(new Dog()
                {
                    OwnerId = RealmService.CurrentUser.Id,
                    Name = summary,
                    Age = 5
                });
            });

            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task GoToDogList()
        {
            var singleton = ObjectSingleton.Instance;
            singleton.SetDogType();
            var loginPage = LoginPage.Instance;
            loginPage.SetDogType();
            //await Shell.Current.Navigation.PushAsync(loginPage);

            await Shell.Current.GoToAsync($"//list_of_dogs");
        }





        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

