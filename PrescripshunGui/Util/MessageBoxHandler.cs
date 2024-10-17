using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace PrescripshunGui.Util;

/// <summary>
/// Handles everything related to MessageBoxes from <seealso cref="MsBox{V,VM,T}"/> / <seealso cref="MessageBoxManager"/>.
/// This way we can create <c>MsBox</c> popups from anywhere, regardless of whether we are on the <seealso cref="Dispatcher.UIThread"/> at the time of
/// a function call, and regardless of whether we have access to any <seealso cref="Control"/>.
///
/// This is useful in case we want to call it from events such as the ones located in <seealso cref="GuiEvents"/>.
/// </summary>
internal static class MessageBoxHandler
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    // TODO: Move to GuiUtil class. Might be useful for other classes!
    /// <summary>
    /// Returns an instance of <seealso cref="Window"/> (or <c>null</c>).
    /// This call can safely be made from all threads.
    /// </summary>
    /// <returns>Returns an instance of <seealso cref="Window"/> unless the <seealso cref="IClassicDesktopStyleApplicationLifetime.MainWindow"/> is <c>null</c>.</returns>
    /// <exception cref="NotImplementedException">When calling from invalid platform.</exception>
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

    /// <summary>
    /// Asynchronously shows a messagebox on the UI Thread.
    /// </summary>
    /// <param name="title">Title to display on top the messagebox.</param>
    /// <param name="text">Text to display in the body of the messagebox.</param>
    /// <param name="buttons"><see langword="enum"/> that decides what buttons to show at the bottom of the messagebox.</param>
    /// <param name="icon"><see langword="enum"/> that decides what icon to show at the top-left of the messagebox.</param>
    /// <param name="location"><see langword="enum"/> that decides the location of the messagebox when it first appears on the screen.</param>
    /// <returns></returns>
    public static async Task SimplePopUp(string title, string text, ButtonEnum buttons = ButtonEnum.Ok, Icon icon = Icon.None, WindowStartupLocation location = WindowStartupLocation.CenterScreen)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, text, buttons, icon, location);
            await box.ShowWindowDialogAsync(GetCurrentOwner());
        });
    }
}