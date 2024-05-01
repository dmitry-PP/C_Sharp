using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp6._2.ViewModel;

namespace WpfApp6._2.View
{
    /// <summary>
    /// Логика взаимодействия для DayPage.xaml
    /// </summary>
    public partial class DayPage : Page
    {
        public DayPage(DateTime date)
        {
            InitializeComponent();
            DataContext = new DayViewModel(date);
        }
    }
}
