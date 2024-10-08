﻿using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using PrescripshunClient;
using PrescripshunGui.Views;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util
{
    internal static class GuiEvents
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ClientEvents GetNetworkEvents() => ClientEvents.Get;

        public static void RegisterEvents()
        {
            NetworkHandler.Client.RegisterEvents();
            Logger.Info("Registering events in {0}", nameof(GuiEvents));

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
                        if (currentWindow is not null) currentWindow.Content = new Dashboard();
                    });
                }
            });
        }
    }
}