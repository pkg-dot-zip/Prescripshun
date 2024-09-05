using System.Net;

namespace PrescripshunLib.Networking
{
    public interface ISocket
    {
        /// <summary>
        /// Sends the <paramref name="text"/> to the server.
        /// </summary>
        /// <param name="text">Text to send to the server.</param>
        void Send(string text);

        /// <summary>
        /// Sends the <paramref name="text"/> to the specified <paramref name="endPoint"/>.
        /// </summary>
        /// <param name="text">The text to send to the specified <paramref name="endPoint"/>.</param>
        /// <param name="endPoint">The endpoint to send the <paramref name="text"/>to.</param>
        public void SendTo(string text, EndPoint endPoint);

        /// <summary>
        /// Starts the receiving process and supplies the retrieved <see cref="ReceivedArgs"/> to the <see cref="IReceiveCallback"/>.<br/>
        /// Should only be called once per endpoint, so once for the server and once per client.
        /// </summary>
        /// <param name="callback">Callback to invoke with the received <see cref="ReceivedArgs"/></param>
        void Receive(IReceiveCallback callback);
    }
}
