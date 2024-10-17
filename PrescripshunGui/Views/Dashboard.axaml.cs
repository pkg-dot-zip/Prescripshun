using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace PrescripshunGui.Views
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void OnChatButtonClick(object sender, RoutedEventArgs e)
        {
            // Toggle visibility of the chat elements inside the ChatContainer
            ChatContainer.IsVisible = !ChatContainer.IsVisible;
        }

        private void OnSendButtonClick(object sender, RoutedEventArgs e)
        {
            // Get the message from the TextBox
            string message = ChatInput.Text;

            // Ensure there is a message to send
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Append the message to the ChatDisplay
                ChatDisplay.Text += $"{DateTime.Now:HH:mm} Me: {message}\n";

                // Clear the input box after sending the message
                ChatInput.Text = string.Empty;

                // Scroll to the bottom of the chat display
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            // Find the ScrollViewer and scroll to the bottom
            var scrollViewer = (ScrollViewer)ChatDisplay.Parent;
            scrollViewer.ScrollToEnd();
        }
    }
}
