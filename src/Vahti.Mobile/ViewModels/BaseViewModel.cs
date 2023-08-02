using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vahti.Mobile.Services;

namespace Vahti.Mobile.ViewModels
{
    /// <summary>
    /// Base class for view models
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy = false;
        private string _title = string.Empty;
        protected INavigationService NavigationService { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }                

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }                
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
            {
                return;
            }                

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
