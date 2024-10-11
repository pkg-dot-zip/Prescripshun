using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.ViewModels.Commands;
using PrescripshunGui.Util;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;
using PrescripshunLib.Util.Faker;

namespace PrescripshunGui.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    public Guid UserKey => NetworkHandler.Client.UserKey;

    private ObservableCollection<IUser> _items = new();

    public ObservableCollection<IUser> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    private ProfileViewModel? _selectedProfileViewModel;
    public ProfileViewModel? SelectedProfileViewModel
    {
        get => _selectedProfileViewModel;
        set => SetProperty(ref _selectedProfileViewModel, value);
    }

    private IMedicalFile _medicalFile;
    public IMedicalFile MedicalFile
    {
        get => _medicalFile;
        set => SetProperty(ref _medicalFile, value);
    }

    public DashboardViewModel()
    {
        Items.AddAll(new FakeHandler().GetDoctors());
    }

    public void OpenProfileView(IUser user)
    {
        SelectedProfileViewModel = new ProfileViewModel(user.Profile, user.MedicalFile);
    }
}