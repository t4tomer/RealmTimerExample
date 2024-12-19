using RealmTodo.Models;

namespace RealmTodo.Views;

public partial class MapsPage : ContentPage
{
	public MapsPage()
	{
        //set the singlton object to mappin type 

        var singleton = ObjectSingleton.Instance;
        singleton.SetMapPinType();
        Console.WriteLine($"current type: {singleton.GetCurrentType().Name}");



        InitializeComponent();
	}
}
