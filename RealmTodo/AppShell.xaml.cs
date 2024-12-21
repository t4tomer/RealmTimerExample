using RealmTodo.Views;
using RealmTodo.Models;

namespace RealmTodo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // to change the type of the list I need to do it through the shell . 
        //var singleton = ObjectSingleton.Instance;
        //singleton.SetItemType();
        //singleton.SetDogType();
        //singleton.SetMapPinType();
        Routing.RegisterRoute("login", typeof(LoginPage));


        Routing.RegisterRoute("itemEdit", typeof(EditItemPage));

        Routing.RegisterRoute("dogEdit", typeof(EditDogPage));

        Routing.RegisterRoute("mapPinEdit", typeof(EditMapPinPage));

        Routing.RegisterRoute("userRecordEdit", typeof(EditUserRecordPage));

        Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));



    }
}

