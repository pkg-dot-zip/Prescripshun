using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace PrescripshunGui.Util
{
    internal static class MessageBoxHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

        public static async Task SimplePopUp(string title, string text, ButtonEnum buttons = ButtonEnum.Ok, Icon icon = Icon.None, WindowStartupLocation location = WindowStartupLocation.CenterScreen)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var box = MessageBoxManager.GetMessageBoxStandard(title, text, buttons, icon, location);
                await box.ShowWindowDialogAsync(GetCurrentOwner());
            });
        }
    }
}
