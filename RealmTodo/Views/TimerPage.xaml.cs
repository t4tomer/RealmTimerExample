using System;
using System.ComponentModel;
using System.Timers;  // Disambiguate Timer reference
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace RealmTodo.Views
{
    public partial class TimerPage : ContentPage, INotifyPropertyChanged
    {
        private static TimerPage _instance; // Singleton instance
        private static readonly object _lock = new object(); // Thread-safety lock

        private System.Timers.Timer _timer;
        private TimeSpan _elapsedTime;
        private bool isRunning = false;

        private string _timerText = "00:00:00"; // Backing field for TimerText

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged(nameof(TimerText)); // Notify the UI when the value changes
            }
        }

        // Private constructor to prevent direct instantiation
        private TimerPage()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(1000);  // 1-second interval
            _timer.Elapsed += OnTimerElapsed;
            BindingContext = this;  // Set binding context to the page itself
        }

        // Public static property to get the singleton instance
        public static TimerPage Instance
        {
            get
            {
                // Ensure thread-safe access
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TimerPage();
                    }
                }
                return _instance;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!isRunning) return;

            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText = _elapsedTime.ToString(@"hh\:mm\:ss"); // Update TimerText property
            });
            Console.WriteLine($"---->(OnTimerElapsed) Time:{_timerText}");
        }

        [RelayCommand]
        public void StartTimer()
        {
            if (!isRunning)
            {
                isRunning = true;
                _timer.Start();
            }
        }

        [RelayCommand]
        public void PauseTimer()
        {
            isRunning = false;
            _timer.Stop();
        }

        [RelayCommand]
        public void ResetTimer()
        {
            isRunning = false;
            _timer.Stop();
            _elapsedTime = TimeSpan.Zero;
            TimerText = "00:00:00"; // Reset TimerText
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
