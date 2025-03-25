using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SchoolJanitor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<(int, double)> _bags;
        private ObservableCollection<string> _trips;
        private int _totalBags;
        private int _currentBagNumber;
        private string _bagWeight;
        private string _tripCount;

        public ObservableCollection<(int, double)> Bags { get => _bags; set { _bags = value; OnPropertyChanged(nameof(Bags)); } }
        public ObservableCollection<string> Trips { get => _trips; set { _trips = value; OnPropertyChanged(nameof(Trips)); } }
        public int TotalBags { get => _totalBags; set { _totalBags = value; OnPropertyChanged(nameof(TotalBags)); } }
        public int CurrentBagNumber { get => _currentBagNumber; set { _currentBagNumber = value; OnPropertyChanged(nameof(CurrentBagNumber)); } }
        public string BagWeight { get => _bagWeight; set { _bagWeight = value; OnPropertyChanged(nameof(BagWeight)); } }
        public string TripCount { get => _tripCount; set { _tripCount = value; OnPropertyChanged(nameof(TripCount)); } }

        public ICommand AddBagCommand { get; }
        public ICommand CalculateTripsCommand { get; }

        public MainViewModel()
        {
            Bags = new ObservableCollection<(int, double)>();
            Trips = new ObservableCollection<string>();
            AddBagCommand = new RelayCommand(AddBag);
            CalculateTripsCommand = new RelayCommand(CalculateTrips);
            CurrentBagNumber = 1;
        }

        private void AddBag()
        {
            if (double.TryParse(BagWeight, out double weight) && weight >= 1.01 && weight <= 3.0 && CurrentBagNumber <= TotalBags)
            {
                Bags.Add((CurrentBagNumber, weight));
                BagWeight = string.Empty;
                CurrentBagNumber++;
            }
        }

        private void CalculateTrips()
        {
            var result = CalculateMinTrips(Bags.Select(b => b.Item2).ToList());
            TripCount = $"Minimum number of trips: {result.Item1}";
            Trips.Clear();
            int tripNumber = 1;
            foreach (var trip in result.Item2)
            {
                Trips.Add($"Trip {tripNumber++}: {string.Join(", ", trip)} kg");
            }
        }

        private (int, List<List<double>>) CalculateMinTrips(List<double> bagWeights)
        {
            List<List<double>> trips = new List<List<double>>();
            bagWeights = bagWeights.OrderByDescending(x => x).ToList();
            while (bagWeights.Count > 0)
            {
                double currentWeight = 0;
                List<double> trip = new List<double>();
                var firstBag = 0;
                while (firstBag < bagWeights.Count && currentWeight + bagWeights[firstBag] <= 3.0)
                {
                    currentWeight += bagWeights[firstBag];
                    trip.Add(bagWeights[firstBag]);
                    bagWeights.RemoveAt(firstBag);
                }
                trips.Add(trip);
            }
            return (trips.Count, trips);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}