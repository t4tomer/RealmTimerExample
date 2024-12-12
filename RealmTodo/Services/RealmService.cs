using System.Text.Json;
using Realms;
using Realms.Sync;
using RealmTodo.Models;
using RealmTodo.ViewModels;

using static Realms.Schema.ObjectSchema;

namespace RealmTodo.Services
{
    public static class RealmService
    {
        private static bool serviceInitialised;

        private static Realms.Sync.App app;

        private static Realm mainThreadRealm;

        private static Realm mainThreadRealmDog;

       private static FlexibleSyncConfiguration config3;


        public static User CurrentUser => app.CurrentUser;

        public static string DataExplorerLink;







        public static async Task Init()
        {
            Console.WriteLine($"---> Init RealmService");


            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("atlasConfig.json");
            using StreamReader reader = new(fileStream);
            var fileContent = await reader.ReadToEndAsync();

            var config = JsonSerializer.Deserialize<RealmAppConfig>(fileContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var appConfiguration = new AppConfiguration(config.AppId)
            {
                BaseUri = new Uri(config.BaseUrl)
            };

            app = Realms.Sync.App.Create(appConfiguration);

            serviceInitialised = true;

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
            DataExplorerLink = config.DataExplorerLink;
            Console.WriteLine($"To view your data in Atlas, use this link: {DataExplorerLink}");
        }


        public static Realm GetMainThreadRealm( )
        {


            
            return mainThreadRealm ??= GetRealm();


        }





        public static Realm GetRealm()
        {

            var singleton = ObjectSingleton.Instance;

            // Default type
            Console.WriteLine($"Default type: {singleton.GetCurrentType().Name}");
            singleton.SetDogType();

            if (singleton.GetCurrentType() == typeof(MapPin))
            {
                Console.WriteLine($"the type is MapPin!!!");

                var configPinMap = new FlexibleSyncConfiguration(app.CurrentUser)
                {
                    PopulateInitialSubscriptions = (realm) =>
                    {
                        var (query, queryName) = GetQueryForSubscriptionMapPinType(realm, SubscriptionType.Mine);
                        realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                    }
                };

                return Realm.GetInstance(configPinMap);

            }
            else if (singleton.GetCurrentType() == typeof(Dog))
            {

                Console.WriteLine($"the type is Dog!!!");

                var configDog = new FlexibleSyncConfiguration(app.CurrentUser)
                {
                    PopulateInitialSubscriptions = (realm) =>
                    {
                        var (query, queryName) = GetQueryForSubscriptionDogType(realm, SubscriptionType.Mine);
                        realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                    }
                };
                return Realm.GetInstance(configDog);
            }

            Console.WriteLine($"the type is Item!!!");

            var configItem = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var (query, queryName) = GetQueryForSubscriptionItemType(realm, SubscriptionType.Mine);
                    realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                }
            };

            return Realm.GetInstance(configItem);
        }

        //public static Realm GetRealm()
        //{
        //    var config = new FlexibleSyncConfiguration(app.CurrentUser)
        //    {
        //        PopulateInitialSubscriptions = (realm) =>
        //        {
        //            var (query, queryName) = GetQueryForSubscriptionType(realm, SubscriptionType.Mine);
        //            realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
        //        }
        //    };

        //    return Realm.GetInstance(config);
        //}
        private static (IQueryable<Item> Query, string Name) GetQueryForSubscriptionType(Realm realm, SubscriptionType subType)
        {
            IQueryable<Item> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Item>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<Item>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }

        public static async Task RegisterAsync(string email, string password)
        {
            await app.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public static async Task LoginAsync(string email, string password)
        {
            await app.LogInAsync(Credentials.EmailPassword(email, password));



            //using var realm = GetRealmForMultipleTypes();
            using var realm = GetRealm();

            await realm.Subscriptions.WaitForSynchronizationAsync();
        }

        public static async Task LogoutAsync()
        {
            await app.CurrentUser.LogOutAsync();
            mainThreadRealm?.Dispose();
            mainThreadRealm = null;
        }

        public static async Task SetSubscription(Realm realm, SubscriptionType subType)
        {
            if (GetCurrentSubscriptionType(realm) == subType)
            {
                return;
            }

            realm.Subscriptions.Update(() =>
            {
                realm.Subscriptions.RemoveAll(true);

                var (query, queryName) = GetQueryForSubscriptionDogType(realm, subType);



                realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
            });

            //There is no need to wait for synchronization if we are disconnected
            if (realm.SyncSession.ConnectionState != ConnectionState.Disconnected)
            {
                await realm.Subscriptions.WaitForSynchronizationAsync();
            }
        }

        public static SubscriptionType GetCurrentSubscriptionType(Realm realm)
        {
            var activeSubscription = realm.Subscriptions.FirstOrDefault();

            return activeSubscription.Name switch
            {
                "all" => SubscriptionType.All,
                "mine" => SubscriptionType.Mine,
                _ => throw new InvalidOperationException("Unknown subscription type")
            };
        }

        // new method-used for adding Item class 
        private static (IQueryable<Item> Query, string Name) GetQueryForSubscriptionItemType(Realm realm, SubscriptionType subType)
        {
            Console.WriteLine($"(GetQueryForSubscriptionItemType)inputObject is Item ");



            IQueryable<Item> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Item>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<Item>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }



        private static (IQueryable<MapPin> Query, string Name) GetQueryForSubscriptionMapPinType(Realm realm, SubscriptionType subType)
        {

            Console.WriteLine($"(GetQueryForSubscriptionDogType)inputObject is MapPin ");


            IQueryable<MapPin> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<MapPin>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<MapPin>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }


        // new method-used for adding Dog class 
        private static (IQueryable<Dog> Query, string Name) GetQueryForSubscriptionDogType(Realm realm, SubscriptionType subType)
        {

            Console.WriteLine($"(GetQueryForSubscriptionDogType)inputObject is Dog ");


            IQueryable<Dog> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Dog>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<Dog>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }
    }

    public enum SubscriptionType
    {
        Mine,
        All,
    }

    public class RealmAppConfig
    {
        public string AppId { get; set; }

        public string BaseUrl { get; set; }


        // If you're getting this app code by cloning the repository at
        // https://github.com/mongodb/template-app-maui-todo, 
        // it does not contain the data explorer link. Download the
        // app template from the Atlas UI to view a link to your data.
        public string DataExplorerLink { get; set; }
    }
}

