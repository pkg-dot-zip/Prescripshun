using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PrescripshunClient;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util
{
    internal static class GuiEvents
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ClientEvents Get() => ClientEvents.Get;

        private static Window? GetCurrentOwner()
        {
            if (Avalonia.Application.Current is null)
            {
                Logger.Trace("Returning null in {0} because no current application running.", nameof(GetCurrentOwner));
                return null;
            }

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            throw new NotImplementedException("No support for other platforms but desktop have been implemented!");
        }

        public static void RegisterEvents()
        {
            NetworkHandler.Client.RegisterEvents();
            Logger.Info("Registering events in {0}", nameof(GuiEvents));

            Get().OnReceiveMessage.AddHandler<LoginResponse>(async (client, message) =>
            {
                if (message.IsValid())
                {
                    Logger.Info("Logged in as {0}", message.Id);
                    return;
                }

                Logger.Info("Login attempt FAILED!");
                string title = "Login failed!";
                string description = message.Reason;

                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(title, description);
                    await box.ShowWindowDialogAsync(GetCurrentOwner());
                });
            });
        }
    }
}