using System;
using System.Collections.ObjectModel;
using Bogus;
using Bogus.DataSets;
using PrescripshunGui.Util;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;
using PrescripshunLib.Util.Faker;

namespace PrescripshunGui.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Guid UserKey => NetworkHandler.Client.UserKey;

    private ObservableCollection<User> _items = [];

    public ObservableCollection<User> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    public DashboardViewModel()
    {
        Items.CollectionChanged += (sender, args) =>
        {
            if (args.NewItems is not null)
            {
                foreach (var argsNewItem in args.NewItems)
                {
                    if (argsNewItem is User newUser)
                    {
                        Logger.Info("Added {0} to observable collection of users.", newUser.Profile.FullName);
                    }
                    else
                    {
                        Logger.Warn("Item added was not a user?! That is impossible?!?!");
                    }
                }
            }
            else
            {
                Logger.Warn("New items were null.");
            }
        };
    }

    private ProfileViewModel? _selectedProfileViewModel;
    public ProfileViewModel? SelectedProfileViewModel
    {
        get => _selectedProfileViewModel;
        set => SetProperty(ref _selectedProfileViewModel, value);
    }
}