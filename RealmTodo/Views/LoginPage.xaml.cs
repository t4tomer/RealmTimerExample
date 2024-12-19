namespace RealmTodo.Views;
using RealmTodo.Models;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        Console.WriteLine($"----@@@@@@@@@@@@@@>(LoginPage)empty constructor");

        InitializeComponent();
        var singleton = ObjectSingleton.Instance;
        //MainPage-second phaze 
        // in order to change the type of the objects that is seen in the list
        // you need to change the singlton type.
        //singleton.SetItemType();
        //singleton.SetDogType();
        singleton.SetMapPinType();


    }

    public void setDogType()
    {
        InitializeComponent();

        var singleton = ObjectSingleton.Instance;
        singleton.SetDogType();



    }


}
