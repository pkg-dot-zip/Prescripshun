using System;
using System.Collections.ObjectModel;
using PrescripshunGui.Util;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.User;
using PrescripshunLib.Util.Faker;

namespace PrescripshunGui.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Guid UserKey => NetworkHandler.Client.UserKey;

    private ObservableCollection<IUser> _items = new ObservableCollection<IUser>();

    public ObservableCollection<IUser> Items
    {
        get { return _items; }
        set { SetProperty(ref _items, value); }
    }

    public DashboardViewModel()
    {
        Items.AddAll(new FakeHandler().GetDoctors());
    }
}