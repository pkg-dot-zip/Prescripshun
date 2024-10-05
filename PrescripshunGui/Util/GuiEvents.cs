using System.Threading.Tasks;
using PrescripshunClient;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util
{
    internal static class GuiEvents
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ClientEvents Get() => ClientEvents.Get;

        public static void RegisterEvents()
        {
            NetworkHandler.Client.RegisterEvents();
            Logger.Info("Registering events in {0}", nameof(GuiEvents));

            Get().OnReceiveMessage.AddHandler<LoginResponse>((client, message) =>
            {
                if (message.IsValid())
                {
                    Logger.Info("Logged in as {0}", message.Id);
                    return Task.CompletedTask;
                }

                Logger.Info("Login attempt FAILED!");
                return Task.CompletedTask;
            });

            Get().OnReceiveMessage.AddHandler<LoginResponse>(async (client, message) =>
            {
                if (!message.IsValid()) await MessageBoxHandler.SimplePopUp("Login Failed!", message.Reason);
            });
        }
    }
}