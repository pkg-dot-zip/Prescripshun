﻿using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using PrescripshunClient;
using PrescripshunGui.ViewModels;
using PrescripshunGui.Views;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util;

internal class GuiEvents
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static ClientEvents GetNetworkEvents() => ClientEvents.Get;

    private static GuiEvents? _instance = null;

    private GuiEvents()
    {
    }

    public static GuiEvents Get => _instance ??= new GuiEvents();


    #region Events
    public delegate Task OnApplicationBootDelegate(string[] args);

    /// <summary>
    /// Event that gets invoked on initiation of the client, before any network code is executed.
    /// </summary>
    public OnApplicationBootDelegate OnApplicationBoot { get; set; } = (args) => Task.CompletedTask;


    public delegate Task OnApplicationExitDelegate(string[] args);

    /// <summary>
    /// Event that gets invoked on exit of the client, after the connection is closed.
    /// </summary>
    public OnApplicationExitDelegate OnApplicationExit { get; set; } = (args) => Task.CompletedTask;
    #endregion

    public void RegisterEvents()
    {
        NetworkHandler.Client.RegisterEvents();
        Logger.Info("Registering events in {0}", nameof(GuiEvents));

        OnApplicationExit += args =>
        {
            NLog.LogManager.Shutdown();
            return Task.CompletedTask;
        };

        OnApplicationBoot += args =>
        {
            Logger.Info($"Starting client at {DateTime.Now} on {Environment.MachineName}.");
            return Task.CompletedTask;
        };

        GetNetworkEvents().OnReceiveMessage.AddHandler<LoginResponse>((client, message) =>
        {
            if (message.IsValid())
            {
                Logger.Info("Logged in as {0}", message.Id);
                NetworkHandler.Client.UserKey = message.Id; // Sets user-key.
                return Task.CompletedTask;
            }

            Logger.Info("Login attempt FAILED!");
            return Task.CompletedTask;
        });

        GetNetworkEvents().OnReceiveMessage.AddHandler<LoginResponse>(async (client, message) =>
        {
            if (message.IsValid()) await MessageBoxHandler.SimplePopUp("TEST SUCCESSFUL!", "CAN LOGIN!"); // TODO: Remove.
            if (!message.IsValid()) await MessageBoxHandler.SimplePopUp("Login Failed!", message.Reason);
        });

        // Close the login window and open the dashboard. 
        GetNetworkEvents().OnReceiveMessage.AddHandler<LoginResponse>(async (client, message) =>
        {
            if (message.IsValid())
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var currentWindow =
                        (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                        ?.MainWindow;
                    if (currentWindow is not null) currentWindow.Content = new Dashboard()
                    {
                        DataContext = new DashboardViewModel()
                    };
                });
            }
        });
    }
}