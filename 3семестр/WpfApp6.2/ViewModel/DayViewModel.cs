using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp6._2.Model;
using WpfApp6._2.View;
using WpfApp6._2.ViewModel.Helpers;

namespace WpfApp6._2.ViewModel
{
    internal class DayViewModel : BindingHelper
    {
        private ObservableCollection<DayInfo> DaysList;
        private DayInfo Day;
        private int DayIndex = -1;

        public BindableCommand Save { get; set; }
        public BindableCommand Delete { get; set; }
        public BindableCommand Back { get; set; }

        public DayViewModel(DateTime date)
        {
            Save = new BindableCommand(_ => SaveChangeDay());
            Delete = new BindableCommand(_ => RemoveDay());
            Back = new BindableCommand(_ => BackPage());


            try
            {
                DaysList = Manager.Deserializer<ObservableCollection<DayInfo>>();
            }
            catch (FileNotFoundException)
            {
                DaysList = new ObservableCollection<DayInfo> { };
            }

            Day = DaysList.FirstOrDefault(x=> x.date == date);

            if (Day == null)
            {
                Day = new DayInfo(date);
            }
            else
            {
                DayIndex = DaysList.IndexOf(Day);
            }
        }


        public void SaveChangeDay()
        {
            if (DayIndex != -1) DaysList[DayIndex] = Day;
            else DaysList.Add(Day);

            Manager.Serializer(DaysList);
        }

        public void RemoveDay()
        {
            DaysList[DayIndex] = new DayInfo(Day.date);

            Manager.Serializer(DaysList);
        }

        public void BackPage()
        {
            MainWindow window = (Application.Current.MainWindow as MainWindow);
            window.PageFrame.Content = new CalendarPage();
        }
    }
}
