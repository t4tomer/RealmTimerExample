namespace RealmTodo.Views;
using RealmTodo.Models;

public partial class ItemsPage : ContentPage
{
	public ItemsPage()
	{
        //set the singlton object to item type 
        var singleton = ObjectSingleton.Instance;
        singleton.SetItemType();
        Console.WriteLine($"current type: {singleton.GetCurrentType().Name}");

        var loginPage = LoginPage.Instance;
        loginPage.SetItemType();

        InitializeComponent();
        singleton.SetItemType();

        loginPage.SetItemType();

    }
}
