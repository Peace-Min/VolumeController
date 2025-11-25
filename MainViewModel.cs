using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace VolumeController
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _volume;
        private bool _isMuted;

        public MainViewModel()
        {
            // Initialize from current system state
            _volume = VolumeService.GetVolume();
            _isMuted = VolumeService.GetMute();
            
            // Initialize command
            PlayBeepCommand = new RelayCommand(ExecutePlayBeep);
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged();
                    VolumeService.SetVolume((float)_volume);
                    OnPropertyChanged(nameof(VolumeText));
                }
            }
        }

        public string VolumeText => $"{(int)Volume}%";

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (_isMuted != value)
                {
                    _isMuted = value;
                    OnPropertyChanged();
                    VolumeService.SetMute(_isMuted);
                }
            }
        }

        private BeepType _selectedBeepType = BeepType.Alert;
        public BeepType SelectedBeepType
        {
            get => _selectedBeepType;
            set
            {
                if (_selectedBeepType != value)
                {
                    _selectedBeepType = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand PlayBeepCommand { get; private set; }

        private void ExecutePlayBeep(object parameter)
        {
            // Play selected beep type
            BeepService.PlayBeep(SelectedBeepType);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly System.Action<object> _execute;
        private readonly System.Func<object, bool> _canExecute;

        public RelayCommand(System.Action<object> execute, System.Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
