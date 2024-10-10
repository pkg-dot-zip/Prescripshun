using System;
using System.Collections.ObjectModel;
using Bogus;
using Bogus.DataSets;
using PrescripshunGui.Util;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.User;
using PrescripshunLib.Util.Faker;

namespace PrescripshunGui.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    public Guid UserKey => NetworkHandler.Client.UserKey;

    private ObservableCollection<IUser> _items = [];


    public ObservableCollection<IUser> ChattableUsers
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    public DashboardViewModel()
    {
    }
}