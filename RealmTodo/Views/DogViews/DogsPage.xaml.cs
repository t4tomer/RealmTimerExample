namespace RealmTodo.Views;
using RealmTodo.Models;

public partial class DogsPage : ContentPage
{
	public DogsPage()
	{

        //set the singlton object to dog type 
        var singleton = ObjectSingleton.Instance;
        singleton.SetDogType();

        var loginPage = LoginPage.Instance;
        loginPage.SetDogType();

        Console.WriteLine($"current type(DogsPage): {singleton.GetCurrentType().Name}");

        InitializeComponent();
        singleton.SetDogType();

        loginPage.SetDogType();
    }
}
