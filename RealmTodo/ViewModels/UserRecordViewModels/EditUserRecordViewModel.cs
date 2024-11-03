using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;

namespace RealmTodo.ViewModels
{
    public partial class EditUserRecordViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        private UserRecord initialUserRecord;

        [ObservableProperty]
        private string summary;

        [ObservableProperty]
        private string mapName;

        [ObservableProperty]
        private string trackTime;

        [ObservableProperty]
        private string uploadDate;

        [ObservableProperty]
        private string pageHeader;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.Count > 0 && query["userrecord"] != null) // Editing an existing UserRecord
            {
                InitialUserRecord = query["userrecord"] as UserRecord;

                Summary = InitialUserRecord.Summary;
                MapName = InitialUserRecord.MapName;
                TrackTime = InitialUserRecord.TrackTime;
                UploadDate = InitialUserRecord.UploadDate;

                PageHeader = $"Modify UserRecord {InitialUserRecord.Id}";
            }
            else // Creating a new UserRecord
            {
                Summary = "RecodSummary";//test
                MapName = "RecordMP_Test";//test
                TrackTime = "TrackTest";
                UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Default timestamp
                PageHeader = "Create a New UserRecord";
            }
        }

        [RelayCommand]
        public async Task SaveUserRecord()
        {

            Console.WriteLine($"SaveUserRecord method!!!");
            UserRecord userRecord = new UserRecord();
            var realm = RealmService.GetMainThreadRealm(userRecord);
            await realm.WriteAsync(() =>
            {
                if (InitialUserRecord != null) // Editing existing UserRecord
                {
                    InitialUserRecord.Summary = Summary;
                    InitialUserRecord.MapName = MapName;
                    InitialUserRecord.TrackTime = TrackTime;
                    InitialUserRecord.UploadDate = UploadDate;
                }
                else // Creating a new UserRecord
                {
                    realm.Add(new UserRecord
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Summary = "summary",
                        MapName = "summary",
                        TrackTime = "summary",
                        UploadDate = "summary"
                    });
                }
            });

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
