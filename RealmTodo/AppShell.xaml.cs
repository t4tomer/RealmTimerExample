﻿using RealmTodo.Views;

namespace RealmTodo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("itemEdit", typeof(EditItemPage));
        Routing.RegisterRoute("userRecordEdit", typeof(EditUserRecordPage));

        Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));



    }
}

