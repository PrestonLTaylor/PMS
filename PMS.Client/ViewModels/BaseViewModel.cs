using PMS.Lib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PMS.Client.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isBusy = false;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value)
                return;

            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }
    public bool IsNotBusy => !_isBusy;

    protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    protected async Task HandleGrpcError(GrpcError error)
    {
        await Shell.Current.DisplayAlert(error.StatusCode.ToString(), $"Error Message: {error.Message}", "OK");
    }
}
