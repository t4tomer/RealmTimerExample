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
    public partial class EditMapPinViewModel : BaseViewModel, IQueryAttributable
    {



        [ObservableProperty]
        private MapPin initialMapPin;

        [ObservableProperty]
        private string summary;


        [ObservableProperty]
        private string mapname;


        [ObservableProperty]
        private string labelpin;


        [ObservableProperty]
        private string address;


        [ObservableProperty]
        private string latitude;


        [ObservableProperty]
        private string longtiude;

        private string currentUserId = RealmService.CurrentUser.Id;


        [ObservableProperty]
        private string pageHeader;

        private static Realms.Sync.App app;


        public EditMapPinViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditMapPinViewModel");



        }




        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.Count > 0 && query["mappin"] != null) // we're editing an Item
            {

                InitialMapPin = query["mappin"] as MapPin;
                Mapname = InitialMapPin.Mapname;
                Labelpin = InitialMapPin.Labelpin;
                Address = InitialMapPin.Address;
                Latitude = InitialMapPin.Latitude;
                Longtiude = InitialMapPin.Longitude;
                PageHeader = $"Modify Map: {InitialMapPin.Mapname}";
            }
            else // we're creating a new pin map
            {
                Mapname = "";
                Labelpin = "";
                Address = "";
                Latitude = "";
                Longtiude = "";

                PageHeader = "Create a New Map";
            }
        }

        //PrintMapNameCommand


      



        [RelayCommand]
        public async Task SaveMapPin()
        {
            Console.WriteLine($"--> SaveMapPin method (MapPinSubscription)");

            //set the singlton object to mapin type 
            var singleton = ObjectSingleton.Instance;
            singleton.SetMapPinType();


            // Get the Realm instance
            var realm = RealmService.GetMainThreadRealm();

            //this check fixed the problem of no flexibale subscrption !!!!

            // Check if the subscription for MapPin  type exists
            var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "MapPinSubscription");

            if (!mapPinSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var mapPinQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(mapPinQuery, new SubscriptionOptions { Name = "MapPinSubscription" });
                });

                Console.WriteLine("MapPin subscription added. Waiting for synchronization...");

                // Wait for synchronization
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("MapPin subscription already exists.");
            }

            // Proceed with adding the Dog object
            await realm.WriteAsync(() =>
            {
                realm.Add(new MapPin()
                {
                    OwnerId = RealmService.CurrentUser.Id,
                    Mapname=summary,
                    Labelpin="labelTest",
                    Address="addrTest",
                    Latitude="lattidueTest",
                    Longitude="longitudeTest"
                });
            });

            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }


        [RelayCommand]
        public async Task GoToMapsPage()
        {

            var singleton = ObjectSingleton.Instance;
            singleton.SetMapPinType();
            //var loginPage = LoginPage.Instance;
            //loginPage.SetItemType();

            await Shell.Current.GoToAsync($"//list_of_maps");





        }





    }
}

