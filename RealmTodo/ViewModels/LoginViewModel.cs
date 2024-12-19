using CommunityToolkit.Mvvm.Input;
using RealmTodo.Services;
using RealmTodo.Models;
namespace RealmTodo.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }


        public LoginViewModel()
        {
            Console.WriteLine($"-->LoginViewModel empty constructor");

        }



        [RelayCommand]
        public async Task OnAppearing()
        {
            await RealmService.Init();

            if (RealmService.CurrentUser != null)
            {
                await GoToMainPage();
            }
        }

        [RelayCommand]
        public async Task Login()
        {
            if (!await VeryifyEmailAndPassword())
            {
                return;
            }

            await DoLogin();
        }

  







        [RelayCommand]
        public async Task SignUp()
        {
            if (!await VeryifyEmailAndPassword())
            {
                return;
            }

            await DoSignup();
        }

        public async Task DoLogin()
        {
            try
            {
                IsBusy = true;
                await RealmService.LoginAsync(Email, Password);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await DialogService.ShowAlertAsync("Login failed", ex.Message, "Ok");
                return;
            }

            await GoToMainPage();
        }

        private async Task DoSignup()
        {
            try
            {
                IsBusy = true;
                await RealmService.RegisterAsync(Email, Password);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await DialogService.ShowAlertAsync("Sign up failed", ex.Message, "Ok");
                return;
            }

            await DoLogin();
        }

        private async Task<bool> VeryifyEmailAndPassword()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await DialogService.ShowAlertAsync("Error", "Please specify both the email and the password", "Ok");
                return false;
            }

            return true;
        }

        private async Task GoToMainPage()
        {


            Console.WriteLine($"----$$$$$$$$$$$$$$$$$$$$$$$$>(LoginViewModel)GoToMainPage");
            var singleton = ObjectSingleton.Instance;

            //first phaze to change the objects
            //that are seen in the list
            //await Shell.Current.GoToAsync($"//list_of_items");
            //await Shell.Current.GoToAsync($"//list_of_dogs");
            await Shell.Current.GoToAsync($"//list_of_maps");





        }


        //private async Task GoToMainPage()
        //{
        //    string setTypeList = "dogs"; // Change this value as needed
        //    var singleton = ObjectSingleton.Instance;

        //    switch (setTypeList.ToLower())
        //    {
        //        case "dogs":
        //            singleton.SetDogType();
        //            await Shell.Current.GoToAsync($"//list_of_dogs");

        //            break;

        //        case "items":
        //            singleton.SetItemType();
        //            await Shell.Current.GoToAsync($"//list_of_items");

        //            break;

        //        case "maps":
        //            singleton.SetMapPinType();
        //            await Shell.Current.GoToAsync($"//list_of_maps");

        //            break;

        //        default:
        //            Console.WriteLine($"Invalid setTypeList value: {setTypeList}");
        //            break;
        //    }
        //}





    }
}

