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
            Console.WriteLine($"-->SaveDog method (EditDogViewModel)");

            //Dog newDog = new Dog();
            ObjectSingleton newObject =  ObjectSingleton.Instance;
            Dog dogType = new Dog();
            newObject.SetObjectType(dogType);
            var realm = RealmService.GetMainThreadRealm(dogType);
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
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

