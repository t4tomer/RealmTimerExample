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

        private string currentUserId = RealmService.CurrentUser.Id;

        // dog class 
        [ObservableProperty]
        private Dog initialDog;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int age;

        // dog class 







        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Console.WriteLine($"-->ApplyQueryAttributes method (EditItemViewModel)");

            if (query.Count > 0 && query["item"] != null) // we're editing an Item
            {
                InitialItem = query["item"] as Item;
                Summary = InitialItem.Summary;
                PageHeader = $"Modify Item {InitialItem.Id}";
            }
            else // we're creating a new item
            {
                Summary = "";
                PageHeader = "Create a New Item";
            }
        }



        [RelayCommand]
        public async Task PrintItemSummary()
        {
            Console.WriteLine("-----> PrintItemSummary method");
            Console.WriteLine($"\n the current user id : {currentUserId}");


            try
            {
                // Get the Realm instance
                var realm = RealmService.GetMainThreadRealm();

                // Fetch all items belonging to the current user
                var items = realm.All<Item>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);

                if (items.Any())
                {
                    Console.WriteLine("Items found in Realm DB:");
                    foreach (var item in items)
                    {
                        Console.WriteLine($"\n - {item.Summary}");
                    }
                }
                else
                {
                    Console.WriteLine("No items found in the Realm database for the current user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching items: {ex.Message}");
            }
        }



        [RelayCommand]
        public async Task SaveItem()
        {
            Console.WriteLine($"-->SaveItem method (EditITemViewModel)");

            object itemType = new Item();
            Item newItem = new Item();


            var realm = RealmService.GetMainThreadRealm();
            //this check fixed the problem of no flexibale subscrption !!!!

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
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Item subscription already exists.");
            }



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

