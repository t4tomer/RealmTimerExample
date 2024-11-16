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

        public static User CurrentUser => app.CurrentUser;

        public static string DataExplorerLink;

        //type of object that is uploaded to the mongodb cloud
        //public static Object inputObject = new Item();//orignal code line


        public static ObjectSingleton newObject2 = ObjectSingleton.Instance;
        public static Object inputObject = newObject2.GetCurrentObjectType();




        public static async Task Init()
        {
            Console.WriteLine($"---> Init RealmService");

            //if (serviceInitialised)
            //{
            //    return;
            //}

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

        public static Realm GetMainThreadRealm(Object newObject)
        {

            inputObject = newObject2.GetCurrentObjectType();
            //inputObject = newObject2.GetCurrentObjectType();
            if (inputObject is Dog)
                Console.WriteLine($"(RealmService-GetMainThreadRealm)inputObject is Dog ");
            else if (inputObject is Item)
                Console.WriteLine($"(RealmService-GetMainThreadRealm)inputObject is Item ");
            else
                Console.WriteLine($"no defined object type");

            //test.DoLogin();

   


            return mainThreadRealm ??= GetRealm(inputObject);

        }

        public static Realm GetRealm(object ObjectType)
        {
            // Ensure the user is logged in before proceeding
            if (app.CurrentUser == null)
            {
                Console.WriteLine("Error: No user is logged in. Please log in first.");
                return null;
            }

            FlexibleSyncConfiguration config;

            // Attempt to create the FlexibleSyncConfiguration
            try
            {
                config = new FlexibleSyncConfiguration(app.CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating FlexibleSyncConfiguration: {ex.Message}");
                return null;
            }

             config = new FlexibleSyncConfiguration(app.CurrentUser);

            if (ObjectType is Dog)
                Console.WriteLine($"(GetRealm method)ObjectType is Dog ");
            else
                Console.WriteLine($"(GetRealm method)ObjectType is Item ");

            if (ObjectType is Dog)
            {

                Console.WriteLine($"Adding Dog object to realm");
                config = new FlexibleSyncConfiguration(app.CurrentUser)
                {
                    PopulateInitialSubscriptions = (realm) =>
                    {
                        //realm.Subscriptions.RemoveAll(true); // Clear previous subscriptions

                        var (query, queryName) = GetQueryForSubscriptionDogType(realm, SubscriptionType.Mine);
                        realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                    }
                };
            }
            else if (ObjectType is Item)
            {
                Console.WriteLine($"Adding Item object to realm");
                config = new FlexibleSyncConfiguration(app.CurrentUser)
                {
                    PopulateInitialSubscriptions = (realm) =>
                    {
                        //realm.Subscriptions.RemoveAll(true); // Clear previous subscriptions

                        var (query, queryName) = GetQueryForSubscriptionItemType(realm, SubscriptionType.Mine);
                        realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                    }
                };
            }

            return Realm.GetInstance(config);
        }

        public static async Task RegisterAsync(string email, string password)
        {
            await app.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public static async Task LoginAsync(string email, string password)
        {
            await app.LogInAsync(Credentials.EmailPassword(email, password));

            //This will populate the initial set of subscriptions the first time the realm is opened
            //Item newItem = new Item();
            //Dog newDog = new Dog();
            inputObject = newObject2.GetCurrentObjectType();

            if (inputObject is Dog)
                Console.WriteLine($"(LoginAsync)inputObject is Dog ");
            else if (inputObject is Item)
                Console.WriteLine($"(LoginAsync)inputObject is Item ");
            else
                Console.WriteLine($"no defined object type");


            using var realm = GetRealm(inputObject);
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

