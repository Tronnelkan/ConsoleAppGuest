using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _welcomeMessage = "Привіт!";

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
