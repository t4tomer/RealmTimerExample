using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using System.Text.Json;
using Realms;
using Realms.Sync;
using RealmTodo.Models;

namespace RealmTodo.ViewModels
{
    public partial class EditItemViewModel : BaseViewModel, IQueryAttributable
    {

        // item class 
        [ObservableProperty]
        private Item initialItem;

        [ObservableProperty]
        private string summary;

        [ObservableProperty]
        private string pageHeader;
        // item class 
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



        [RelayCommand]
        public async Task AddDog()
        {
            app = Realms.Sync.App.Create("atlastest-qmfoisb"); // Replace with your Realm App ID
            Console.WriteLine("Realm App initialized successfully.");

            // Ensure the app and user are properly initialized
            if (app == null)
            {
                Console.WriteLine("Error: Realm app is not initialized.");
                return;
            }


            var config = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    // Subscribe to all Dog objects, or apply a valid filter.
                    var myItems = realm.All<Dog>().Where(d => d.Age > 5);
                    realm.Subscriptions.Add(myItems);
                }
            };

            // Get Realm instance
            var realm = await Realm.GetInstanceAsync(config);
            Console.WriteLine("Realm instance created successfully.");

            // Update subscriptions
            realm.Subscriptions.Update(() =>
            {
                var myDogs = realm.All<Dog>().Where(t => t.Name == "Clifford" && t.Age > 5);
                if (myDogs.Any())
                {
                    realm.Subscriptions.Add(myDogs);
                    Console.WriteLine("Subscription added.");
                }
                else
                {
                    Console.WriteLine("Warning: No matching dogs found for subscription.");
                }
            });

            await realm.Subscriptions.WaitForSynchronizationAsync();
            Console.WriteLine("Subscription synchronized successfully.");

            // Write block to add a new Dog object
            realm.Write(() =>
            {
                var newDog = new Dog { Name = Summary, Age = 10 };
                Console.WriteLine($"Adding dog: {newDog.Name}, Age: {newDog.Age}");
                realm.Add(newDog);
                Console.WriteLine("Dog added successfully.");
            });

            // Verify that the dog was added
            var dogs = realm.All<Dog>().ToList();
            Console.WriteLine($"Total dogs in database: {dogs.Count}");
        }



        // ApplyQueryAttributes-dog class 
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
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
            Dog newDog = new Dog();
            var realm = RealmService.GetMainThreadRealm(newDog);
            await realm.WriteAsync(() =>
            {
                if (InitialDog != null) // editing an item
                {
                    //InitialItem.Summary = Summary;
                    InitialDog.Name = Name;
                    InitialDog.Age = Age;
                }
                else // creating a new item
                {
                    realm.Add(new Dog()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Name = summary,
                        Age = 0
                    });
                }
            });

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }

        // ApplyQueryAttributes -item class
        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    if (query.Count > 0 && query["item"] != null) // we're editing an Item
        //    {
        //        InitialItem = query["item"] as Item;
        //        Summary = InitialItem.Summary;
        //        PageHeader = $"Modify Item {InitialItem.Id}";
        //    }
        //    else // we're creating a new item
        //    {
        //        Summary = "";
        //        PageHeader = "Create a New Item";
        //    }
        //}

        [RelayCommand]
        public async Task SaveItem()
        {
            Item newItem = new Item();
            var realm = RealmService.GetMainThreadRealm(newItem);
            await realm.WriteAsync(() =>
            {
                if (InitialItem != null) // editing an item
                {
                    InitialItem.Summary = Summary;
                }
                else // creating a new item
                {
                    realm.Add(new Item()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Summary = summary
                    });
                }
            });

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

