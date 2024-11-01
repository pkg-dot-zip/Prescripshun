using Avalonia.Controls;
using Avalonia.Interactivity;
using PrescripshunGui.ViewModels;
using PrescripshunLib.Models.User;

namespace PrescripshunGui.Views
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void OpenProfileView(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: User user } && DataContext is DashboardViewModel viewModel)
            {
                 viewModel.OpenProfileView(user);
            }
        }
    }
}