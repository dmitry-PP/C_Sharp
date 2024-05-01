using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfApp6._2.Model;
using WpfApp6._2.ViewModel.Helpers;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using WpfApp6._2.View;
using Application = System.Windows.Application;

namespace WpfApp6._2.ViewModel
{
    internal class MainViewModel : BindingHelper
    {
        private ObservableCollection<DayCard> DaysList;
        private DayCard selected;
        private DateTime currentDate;
        private MainWindow window;

        public BindableCommand Next { get; set; }
        public BindableCommand Back { get; set; }

        public MainViewModel(MainWindow window)
        {
            this.window = window;

            Days = new ObservableCollection<DayCard>();
            currentDate = DateTime.Today;

            Next = new BindableCommand(_ => NextMonth());
            Back = new BindableCommand(_ => PreviousMonth());

            UpdateCalendar();

        }

        private void UpdateCalendar()
        {
            MonthYearDisplay = currentDate.ToString("MMMM yyyy");

            Days.Clear();
            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            int days = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);


            for (int i = 1; i <= days; i++)
            {
                Days.Add(new DayCard ( i.ToString() ));
            }
        }

        public void PreviousMonth()
        {
            CurrentMonth = currentDate.AddMonths(-1);
        }

        public void NextMonth()
        {
            CurrentMonth = currentDate.AddMonths(1);
        }


        public void OpenDay(string dayNumber)
        {
            DateTime day = new DateTime(currentDate.Year,currentDate.Month, int.Parse(dayNumber));

            MainWindow window = (Application.Current.MainWindow as MainWindow);
            window.PageFrame.Content = new DayPage(day);

        }

        public string MonthYearDisplay
        {
            get { return CurrentMonth.ToString("MMMM yyyy"); }
            set
            {
                OnPropertyChanged();
            }
        }

        public DateTime CurrentMonth
        {
            get { return currentDate; }
            set { 
                currentDate = value;
                OnPropertyChanged();
                UpdateCalendar();
                
            }
        }

        public ObservableCollection<DayCard> Days
        {
            get { return DaysList; }
            set
            {
                DaysList = value;
                OnPropertyChanged();
            }
        }

        public DayCard Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OpenDay(Selected.DayNumber);
                OnPropertyChanged();
            }
        }
    }

}
