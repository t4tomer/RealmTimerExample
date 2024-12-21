namespace RealmTodo.Views;

using RealmTodo.Models;

public partial class LoginPage : ContentPage
{
    // Static instance of LoginPage
    private static LoginPage _instance;

    // Lock object for thread safety
    private static readonly object _lock = new();

    // Private constructor to prevent direct instantiation
    private LoginPage()
    {
        Console.WriteLine($"----@@@@@@@@@@@@@@>(LoginPage)empty constructor");

        InitializeComponent();

        var singleton = ObjectSingleton.Instance;
        // 2 phaze to change the type of the object 
        //singleton.SetItemType();
        //singleton.SetDogType();
        singleton.SetMapPinType();
    }

    // Public property to access the singleton instance
    public static LoginPage Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) // Ensure thread safety
                {
                    if (_instance == null)
                    {
                        _instance = new LoginPage();
                    }
                }
            }
            return _instance;
        }
    }

    // Method to set the singleton type to Dog
    public void SetDogType()
    {
        var singleton = ObjectSingleton.Instance;
        singleton.SetDogType();
    }

    public void SetItemType()
    {
        var singleton = ObjectSingleton.Instance;
        singleton.SetItemType();
    }

}
