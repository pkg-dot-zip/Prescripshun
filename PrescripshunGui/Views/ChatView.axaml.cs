using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace PrescripshunGui.Views
{
    public partial class ChatView : UserControl
    {
        private TextBox? _chatInput;
        private TextBlock? _chatDisplay;
        private ScrollViewer? _chatScrollViewer;

        public ChatView()
        {
            InitializeComponent();

            _chatInput = this.FindControl<TextBox>("ChatInput");
            _chatDisplay = this.FindControl<TextBlock>("ChatDisplay");
            _chatScrollViewer = this.FindControl<ScrollViewer>("ChatScrollViewer");
        }

        private void OnSendButtonClick(object sender, RoutedEventArgs e)
        {
            if (_chatInput == null || _chatDisplay == null) return;

            string message = _chatInput.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                _chatDisplay.Text += $"{DateTime.Now:HH:mm} Me: {message}\n";
                _chatInput.Text = string.Empty;
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            _chatScrollViewer?.ScrollToEnd();
        }
    }
}

// Dashboard.axaml.cs


namespace PrescripshunGui.Views
{
    public partial class Dashboard : UserControl
    {
        private ChatView? _chatView;

        public Dashboard()
        {
            InitializeComponent();
            _chatView = this.FindControl<ChatView>("ChatView");
        }

        private void OnChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (_chatView != null)
            {
                var chatContainer = _chatView.FindControl<Grid>("ChatContainer");
                if (chatContainer != null)
                {
                    chatContainer.IsVisible = !chatContainer.IsVisible;
                }
            }
        }
    }
}