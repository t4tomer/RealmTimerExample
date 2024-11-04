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






        //// ApplyQueryAttributes-dog class 
        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    if (query.Count > 0 && query["dog"] != null) // we're editing an Item
        //    {
        //        InitialDog = query["dog"] as Dog;
        //        Name = InitialDog.Name;
        //        Age = InitialDog.Age;
        //        PageHeader = $"Modify Dog {InitialDog.Id}";
        //    }
        //    else // we're creating a new item
        //    {
        //        Name = "nameTest";
        //        Age = 0;
        //    }
        //}

         //ApplyQueryAttributes -item class
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Console.WriteLine($"-->ApplyQueryAttributes method (EditITemViewModel)");

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



        [RelayCommand]
        public async Task SaveItem()
        {
            Console.WriteLine($"-->SaveItem method (EditITemViewModel)");


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

