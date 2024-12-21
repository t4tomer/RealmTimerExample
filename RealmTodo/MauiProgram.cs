﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
namespace RealmTodo;
using RealmTodo.Models;
using RealmTodo.Views;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {


        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
           
            });


#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddSingleton(LoginPage.Instance);

#endif

        return builder.Build();
    }
}

